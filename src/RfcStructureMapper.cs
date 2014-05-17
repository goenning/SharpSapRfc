
using SAP.Middleware.Connector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SharpSapRfc
{
    internal class RfcStructureMapper
    {
        private static IDictionary<Type, IDictionary<string, PropertyInfo>> typeProperties = new Dictionary<Type, IDictionary<string, PropertyInfo>>();

        private void EnsureTypeIsCached(Type type)
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

        public IRfcTable CreateTable(RfcTableMetadata metadata, object parameterObject)
        {
            IEnumerable  enumerable = parameterObject as IEnumerable;
            if (enumerable == null)
                return null;

            IRfcTable table = metadata.CreateTable();
            RfcStructureMetadata structureMetadata = metadata.LineType;
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                IRfcStructure row = this.CreateStructure(structureMetadata, current);
                table.Append(row);
            }
            return table;
        }

        public IRfcStructure CreateStructure(RfcStructureMetadata metadata, object parameterObject)
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
                    {
                        value = AbapBool.ToString((Boolean)value);
                    }
                    structure.SetValue(fieldName, value);
                }
            }
            return structure;
        }

        public T FromStructure<T>(IRfcStructure structure)
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
                    if (property.PropertyType.Equals(typeof(Boolean)))
                    {
                        value = AbapBool.FromString(value.ToString());
                    }

                    property.SetValue(returnValue, value, null);
                }
            }
            return returnValue;
        }
    }
}
