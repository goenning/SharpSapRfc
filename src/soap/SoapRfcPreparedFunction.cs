using SharpSapRfc.Metadata;
using SharpSapRfc.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace SharpSapRfc.Soap
{
    public class SoapRfcPreparedFunction : RfcPreparedFunction
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

        private SapSoapRfcWebClient webClient;
        private FunctionMetadata function;

        public SoapRfcPreparedFunction(FunctionMetadata function, SapSoapRfcWebClient webClient)
        {
            this.function = function;
            this.webClient = webClient;
        }

        public override RfcResult Execute()
        {
            XmlDocument body = new XmlDocument();
            XmlNode parametersNode = body.CreateElement("urn", this.function.Name, "urn:sap-com:document:sap:rfc:functions");
            body.AppendChild(parametersNode);

            foreach (var parameter in this.parameters)
            {
                var param = this.function.GetInputParameter(parameter.Name);
                if (parameter.Value != null)
                {
                    switch (param.DataType)
                    {
                        case AbapDataType.STRUCTURE:
                            XmlNode structureNode = body.CreateElement(param.Name.ToUpper());
                            FillElementFromStructure(body, structureNode, param.StructureMetadata, parameter.Value);
                            parametersNode.AppendChild(structureNode);
                            break;
                        case AbapDataType.TABLE:
                            XmlNode tableNode = body.CreateElement(param.Name.ToUpper());
                            var items = body.CreateElement("item");

                            IEnumerable enumerable = parameter.Value as IEnumerable;
                            if (enumerable == null)
                            {
                                XmlNode itemNode = body.CreateElement(param.Name.ToUpper());
                                FillElementFromStructure(body, itemNode, param.StructureMetadata, parameter.Value);
                                items.AppendChild(itemNode);
                            }
                            else 
                            { 
                                var enumerator = enumerable.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    object current = enumerator.Current;
                                    XmlNode itemNode = body.CreateElement(param.Name.ToUpper());
                                    FillElementFromStructure(body, itemNode, param.StructureMetadata, parameter.Value);
                                    items.AppendChild(itemNode);
                                }
                            }

                            tableNode.InnerText = AbapValueMapper.ToRemoteValue(param.DataType, parameter.Value).ToString();
                            tableNode.AppendChild(items);
                            parametersNode.AppendChild(tableNode);
                            break;
                        default:
                            XmlNode valueNode = body.CreateElement(param.Name.ToUpper());
                            valueNode.InnerText = AbapValueMapper.ToRemoteValue(param.DataType, parameter.Value).ToString();
                            parametersNode.AppendChild(valueNode);
                            break;
                    }
                }
            }

            var responseXml = this.webClient.SendRfcRequest(this.function.Name, body);

            var responseTag = responseXml.GetElementsByTagName(string.Format("urn:{0}.Response", this.function.Name));
            if (responseTag.Count > 0)
                return new SoapRfcResult(this.function, responseTag[0]);

            var exceptionTag = responseXml.GetElementsByTagName(string.Format("rfc:{0}.Exception", this.function.Name));
            if (exceptionTag.Count > 0)
                throw new RfcException(exceptionTag[0].InnerText);

            var faultErrorTag = responseXml.GetElementsByTagName("rfc:Error");
            if (faultErrorTag.Count > 0)
            {
                string errorText = faultErrorTag[0].SelectSingleNode("message").InnerText;
                throw new RfcException(errorText);
            }

            throw new Exception("Could not fetch response tag.");
        }

        private void FillElementFromStructure(XmlDocument document, XmlNode node, StructureMetadata metadata, object parameterObject)
        {
            Type type = parameterObject.GetType();
            EnsureTypeIsCached(type);

            foreach (var field in metadata.Fields)
            {
                XmlElement parameterNode = document.CreateElement(field.Name);
                PropertyInfo property = null;

                object formattedValue = null;
                if (typeProperties[type].TryGetValue(field.Name.ToLower(), out property))
                {
                    object value = property.GetValue(parameterObject, null);
                    formattedValue = AbapValueMapper.ToRemoteValue(field.DataType, value);
                }
                else if (string.IsNullOrEmpty(field.Name))
                    formattedValue = AbapValueMapper.ToRemoteValue(field.DataType, parameterObject);

                parameterNode.Value = formattedValue.ToString();

            }
        }
    }
}
