
using SAP.Middleware.Connector;
using SharpSapRfc.Structure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace SharpSapRfc
{
    internal class RfcStructureMapper
    {
        private static IDictionary<Type, IDictionary<string, PropertyInfo>> typeProperties = new Dictionary<Type, IDictionary<string, PropertyInfo>>();
        private static CultureInfo enUS = new CultureInfo("en-US");

        private static void EnsureTypeIsCached(Type type)
        {
            if (typeProperties.ContainsKey(type))
                return;

            lock (type.GetType())
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
                if (typeProperties[type].TryGetValue(fieldName.ToLower(), out property))
                {
                    object value = property.GetValue(parameterObject, null);
                    if (property.PropertyType.Equals(typeof(Boolean)))
                        value = AbapBool.ToString((Boolean)value);
                    else if (metadata[i].DataType == RfcDataType.DATE)
                        value = AbapDateTime.ToString((DateTime)value, AbapDateTimeType.Date);
                    else if (metadata[i].DataType == RfcDataType.TIME)
                        value = AbapDateTime.ToString((DateTime)value, AbapDateTimeType.Time);
                    structure.SetValue(fieldName, value);
                }
            }
            return structure;
        }

        public static T FromStructure<T>(IRfcStructure structure)
        {
            Type type = typeof(T);
            EnsureTypeIsCached(type);

            T returnValue = Activator.CreateInstance<T>();
            for (int i = 0; i < structure.Metadata.FieldCount; i++)
            {
                string fieldName = structure.Metadata[i].Name;
                PropertyInfo property = null;
                if (typeProperties[type].TryGetValue(fieldName.ToLower(), out property))
                {
                    object value = structure.GetValue(fieldName);
                    value = RfcStructureMapper.FromValue(property.PropertyType, value);
                    property.SetValue(returnValue, value, null);
                }
            }
            return returnValue;
        }

        public static object FromValue(Type targetType, object value)
        {
            if (targetType.Equals(typeof(Boolean)))
                value = AbapBool.FromString(value.ToString());
            else if (targetType.Equals(typeof(DateTime)))
                value = AbapDateTime.FromString(value.ToString());
            else if (targetType.Equals(typeof(Int32)))
                value = Convert.ToInt32(value);
            return value;
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

                        object targetValue = null;
                        if (property.PropertyType.Equals(typeof(Boolean)))
                            targetValue = AbapBool.FromString(value.ToString());
                        else if (property.PropertyType.Equals(typeof(DateTime)))
                            targetValue = AbapDateTime.FromString(value.ToString());
                        else if (field.Type == "P")
                            targetValue = Convert.ToDecimal(value, enUS.NumberFormat);
                        else
                            targetValue = Convert.ChangeType(value, property.PropertyType);
                        property.SetValue(entry, targetValue, null);
                    }
                }
                entries.Add(entry);
            }
            return entries;
        }
    }
}
