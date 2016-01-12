
using SharpSapRfc.Metadata;
using System;
using System.Collections;
using System.Reflection;
using System.Xml;

namespace SharpSapRfc.Soap
{
    public class SoapRfcStructureMapper : RfcStructureMapper
    {
        public SoapRfcStructureMapper(RfcValueMapper valueMapper)
            : base(valueMapper)
        {
        }


        public T FromXml<T>(StructureMetadata metadata, XmlNode node)
        {
            Type type = typeof(T);
            EnsureTypeIsCached(type);

            T returnObject = default(T);
            if (!type.Equals(typeof(string)))
                returnObject = Activator.CreateInstance<T>();

            if (metadata == null)
                return (T)this.FromRemoteValue(type, node.InnerText);

            foreach (var field in metadata.Fields)
	        {
                string fieldName = field.Name;
                XmlNode valueNode = node.SelectSingleNode(fieldName);
                PropertyInfo property = null;
                if (typeProperties[type].TryGetValue(fieldName.ToLower(), out property))
                    SetProperty(returnObject, property, valueNode.InnerXml);
            }
            return returnObject;
        }

        public XmlNode FromStructure(XmlDocument body, string nodeName, FieldMetadata param, object parameterObject)
        {
            XmlNode node = body.CreateElement(nodeName);
            Type type = parameterObject.GetType();
            EnsureTypeIsCached(type);

            if (param.StructureMetadata == null)
            {
                node.InnerText = this.ToRemoteValue(param.DataType, parameterObject).ToString();
            }
            else
            {
                foreach (var field in param.StructureMetadata.Fields)
                {
                    XmlElement parameterNode = body.CreateElement(field.Name);
                    PropertyInfo property = null;

                    object formattedValue = null;
                    if (typeProperties[type].TryGetValue(field.Name.ToLower(), out property))
                    {
                        object value = property.GetValue(parameterObject, null);
                        formattedValue = this.ToRemoteValue(field.DataType, value);
                    }
                    else if (string.IsNullOrEmpty(field.Name))
                        formattedValue = this.ToRemoteValue(field.DataType, parameterObject);

                    if (formattedValue != null)
                    {
                        parameterNode.InnerText = (formattedValue ?? string.Empty).ToString();
                        if (!string.IsNullOrEmpty(parameterNode.InnerText))
                            node.AppendChild(parameterNode);
                    }
                }
            }
            return node;
        }

        protected override IList FromRemoteTable(Type type, object remoteValue)
        {
            throw new NotImplementedException();
        }

        protected override object FromRemoteStructure(Type type, object remoteValue)
        {
            throw new NotImplementedException();
        }
    }
}
