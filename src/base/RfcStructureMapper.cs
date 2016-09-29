using SharpSapRfc.Structure;
using SharpSapRfc.Types;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpSapRfc
{
    public abstract class RfcStructureMapper
    {
        private RfcValueMapper valueMapper;
        public RfcStructureMapper(RfcValueMapper valueMapper)
        {
            this.valueMapper = valueMapper;
        }

        protected static IDictionary<Type, IDictionary<string, PropertyInfo>> typeProperties = new Dictionary<Type, IDictionary<string, PropertyInfo>>();

        protected void EnsureTypeIsCached(Type type)
        {
            if (typeProperties.ContainsKey(type))
                return;

            lock (type)
            {
                if (typeProperties.ContainsKey(type))
                    return;

                IDictionary<string, PropertyInfo> propertyByFieldName = new Dictionary<string, PropertyInfo>();

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

        public IEnumerable<T> FromRfcReadTableToList<T>(IEnumerable<Tab512> table, IEnumerable<RfcDbField> fields)
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
                            value = row.Data.Substring(field.Offset).Trim();
                        else
                            value = row.Data.Substring(field.Offset, field.Length).Trim();

                        SetProperty(entry, property, value);
                    }
                }
                entries.Add(entry);
            }
            return entries;
        }

        protected void SetProperty(object targetObject, PropertyInfo property, object remoteValue)
        {
            object formattedValue = this.valueMapper.FromRemoteValue(property.PropertyType, remoteValue);

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

        public object FromRemoteValue(Type type, object value)
        {
            return this.valueMapper.FromRemoteValue(type, value);
        }

        public object ToRemoteValue(AbapDataType remoteType, object value)
        {
            return this.valueMapper.ToRemoteValue(remoteType, value);
        }
    }
}
