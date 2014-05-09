using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpSapRfc
{
    public class RfcResult
    {
        private static IDictionary<Type, IDictionary<string, PropertyInfo>> typeProperties = new Dictionary<Type, IDictionary<string, PropertyInfo>>();

        private IRfcFunction function;

        public RfcResult(IRfcFunction function)
        {
            this.function = function;
        }

        public T GetExportParameter<T>(string name)
        {
            object returnValue = function.GetValue(name);
            if (returnValue is IRfcStructure) 
            {
                return this.ConvertFromStructure<T>(returnValue as IRfcStructure);
            }

            return (T)Convert.ChangeType(function.GetValue(name), typeof(T));
        }

        private T ConvertFromStructure<T>(IRfcStructure structure)
        {
            Type type = typeof(T);
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
                        propertyByFieldName.Add(attribute.FieldName, property);
                    }
                }
                typeProperties.Add(type, propertyByFieldName);
            }

            T returnValue = Activator.CreateInstance<T>();
            for (int i = 0; i < structure.Metadata.FieldCount; i++)
            {
                string fieldName = structure.Metadata[i].Name;
                PropertyInfo property = null;
                if (propertyByFieldName.TryGetValue(fieldName, out property))
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

        public T[] GetExportTable<T>(string name)
        {
            IRfcTable table = this.function.GetTable(name);
            T[] returnTable = new T[table.RowCount];
            for (int i = 0; i < table.RowCount; i++)
            {
                returnTable[i] = ConvertFromStructure<T>(table[i]);
            }
            return returnTable;
        }
    }
}
