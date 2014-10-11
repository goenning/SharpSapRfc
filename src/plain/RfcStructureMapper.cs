
using SAP.Middleware.Connector;
using SharpSapRfc.Structure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace SharpSapRfc.Plain
{
    internal class RfcStructureMapper
    {
        private static IDictionary<Type, IDictionary<string, PropertyInfo>> typeProperties = new Dictionary<Type, IDictionary<string, PropertyInfo>>();
        private static CultureInfo enUS = new CultureInfo("en-US");

        private static void EnsureTypeIsCached(Type type)
        {
            if (typeProperties.ContainsKey(type))
                return;

            lock (type)
            {
                if (typeProperties.ContainsKey(type))
                    return;

                IDictionary<string, PropertyInfo> propertyByFieldName = new Dictionary<string, PropertyInfo>();
                if (typeProperties.ContainsKey(type))
                {
                    propertyByFieldName = typeProperties[type];
                }
                else
                {
                    PropertyInfo[] properties = type.GetProperties();
                    foreach (var property in properties)
                    {
                        if (property.IsDefined(typeof(RfcStructureFieldAttribute), true))
                        {
                            var attribute = ((RfcStructureFieldAttribute[])property.GetCustomAttributes(typeof(RfcStructureFieldAttribute), true))[0];
                            propertyByFieldName.Add(attribute.FieldName.ToLower(), property);
                            if (!string.IsNullOrWhiteSpace(attribute.SecondFieldName))
                                propertyByFieldName.Add(attribute.SecondFieldName.ToLower(), property);
                        }
                        else
                            propertyByFieldName.Add(property.Name.ToLower(), property);
                    }
                    typeProperties.Add(type, propertyByFieldName);
                }
            }
        }

        public static IRfcTable CreateTable(RfcTableMetadata metadata, object parameterObject)
        {

            IRfcTable table = metadata.CreateTable();
            RfcStructureMetadata structureMetadata = metadata.LineType;

            IEnumerable enumerable = parameterObject as IEnumerable;
            if (enumerable == null)
            {
                IRfcStructure row = CreateStructure(structureMetadata, parameterObject);
                table.Append(row);
            }
            else 
            { 
                var enumerator = enumerable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    IRfcStructure row = CreateStructure(structureMetadata, current);
                    table.Append(row);
                }
            }
            return table;
        }

        public static IRfcStructure CreateStructure(RfcStructureMetadata metadata, object parameterObject)
        {
            if (parameterObject == null)
                return null;

            IRfcStructure structure = metadata.CreateStructure();
            Type type = parameterObject.GetType();
            EnsureTypeIsCached(type);

            for (int i = 0; i < metadata.FieldCount; i++)
            {
                string fieldName = metadata[i].Name;
                PropertyInfo property = null;
                
                object formattedValue = null;
                if (typeProperties[type].TryGetValue(fieldName.ToLower(), out property))
                {
                    object value = property.GetValue(parameterObject, null);
                    formattedValue = AbapValueMapper.ToRemoteValue(metadata[i].GetAbapDataType(), value);
                }
                else if (string.IsNullOrEmpty(fieldName))
                    formattedValue = AbapValueMapper.ToRemoteValue(metadata[i].GetAbapDataType(), parameterObject);

                structure.SetValue(fieldName, formattedValue);
            }
            return structure;
        }

        public static T FromStructure<T>(IRfcStructure structure)
        {
            Type type = typeof(T);
            EnsureTypeIsCached(type);

            T returnObject = default(T);
            if (!type.Equals(typeof(string)))
                returnObject= Activator.CreateInstance<T>();

            for (int i = 0; i < structure.Metadata.FieldCount; i++)
            {
                string fieldName = structure.Metadata[i].Name;
                object value = structure.GetValue(fieldName);
                if (string.IsNullOrEmpty(fieldName))
                    return (T)AbapValueMapper.FromRemoteValue(type, value);

                PropertyInfo property = null;
                if (typeProperties[type].TryGetValue(fieldName.ToLower(), out property))
                    SetProperty(returnObject, property, value);
            }
            return returnObject;
        }

        public static void SetProperty(object targetObject, PropertyInfo property, object remoteValue)
        {
            object formattedValue = AbapValueMapper.FromRemoteValue(property.PropertyType, remoteValue);

            if (property.PropertyType == typeof(DateTime) ||
                property.PropertyType == typeof(DateTime?))
            {
                DateTime? formattedDateTimeValue = (DateTime?)formattedValue;
                DateTime? actualValue = (DateTime?)property.GetValue(targetObject, null);
                if (actualValue.HasValue && formattedDateTimeValue.HasValue && actualValue.Value != DateTime.MinValue)
                    formattedValue = actualValue.Value.AddTicks(formattedDateTimeValue.Value.Ticks);
            }

            property.SetValue(targetObject, formattedValue, null);
        }

        public static IEnumerable<T> FromRfcReadTableToList<T>(IEnumerable<Tab512> table, IEnumerable<RfcDbField> fields)
        {
            Type type = typeof(T);
            EnsureTypeIsCached(type);

            List<T> entries = new List<T>();
            foreach (var row in table)
            {
                T entry = Activator.CreateInstance<T>();
                foreach (var field in fields)
                {
                    PropertyInfo property = null;
                    if (typeProperties[type].TryGetValue(field.FieldName.ToLower(), out property))
                    {
                        string value = null;
                        if (field.Offset >= row.Data.Length)
                            value = string.Empty;
                        else if (field.Length + field.Offset > row.Data.Length)
                            value = row.Data.Substring(field.Offset).TrimEnd();
                        else
                            value = row.Data.Substring(field.Offset, field.Length).TrimEnd();

                        SetProperty(entry, property, value);
                    }
                }
                entries.Add(entry);
            }
            return entries;
        }
    }
}
