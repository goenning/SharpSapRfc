
using SAP.Middleware.Connector;
using System;
using System.Collections;
using System.Reflection;

namespace SharpSapRfc.Plain
{
    public class PlainRfcStructureMapper : RfcStructureMapper
    {
        public PlainRfcStructureMapper(RfcValueMapper valueMapper)
            : base(valueMapper)
        {

        }

        public IRfcTable CreateTable(RfcTableMetadata metadata, object parameterObject)
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
                
                object formattedValue = null;
                if (typeProperties[type].TryGetValue(fieldName.ToLower(), out property))
                {
                    object value = property.GetValue(parameterObject, null);
                    formattedValue = this.ToRemoteValue(metadata[i].GetAbapDataType(), value);
                }
                else if (string.IsNullOrEmpty(fieldName))
                    formattedValue = this.ToRemoteValue(metadata[i].GetAbapDataType(), parameterObject);

                structure.SetValue(fieldName, formattedValue);
            }
            return structure;
        }

        public T FromStructure<T>(IRfcStructure structure)
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
                    return (T)this.FromRemoteValue(type, value);

                PropertyInfo property = null;
                if (typeProperties[type].TryGetValue(fieldName.ToLower(), out property))
                    this.SetProperty(returnObject, property, value);
            }
            return returnObject;
        }
    }
}
