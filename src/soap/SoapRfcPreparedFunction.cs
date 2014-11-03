using SharpSapRfc.Metadata;
using SharpSapRfc.Types;
using System;
using System.Collections;
using System.Text;
using System.Xml;

namespace SharpSapRfc.Soap
{
    public class SoapRfcPreparedFunction : RfcPreparedFunction
    {
        private SoapRfcWebClient webClient;
        private FunctionMetadata function;
        private SoapRfcStructureMapper structureMapper;

        public SoapRfcPreparedFunction(FunctionMetadata function, SoapRfcStructureMapper structureMapper, SoapRfcWebClient webClient)
        {
            this.function = function;
            this.webClient = webClient;
            this.structureMapper = structureMapper;
        }

        public override RfcResult Execute()
        {
            XmlDocument body = new XmlDocument();
            body.PreserveWhitespace = true;
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
                            XmlNode structureNode = this.structureMapper.FromStructure(body, param.Name, param, parameter.Value);
                            parametersNode.AppendChild(structureNode);
                            break;
                        case AbapDataType.TABLE:
                            XmlNode tableNode = body.CreateElement(param.Name.ToUpper());

                            IEnumerable enumerable = parameter.Value as IEnumerable;
                            if (enumerable == null)
                            {
                                XmlNode itemNode = this.structureMapper.FromStructure(body, "item", param, parameter.Value);
                                tableNode.AppendChild(itemNode);
                            }
                            else 
                            { 
                                var enumerator = enumerable.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    object current = enumerator.Current;
                                    XmlNode itemNode = this.structureMapper.FromStructure(body, "item", param, current); 
                                    tableNode.AppendChild(itemNode);
                                }
                            }

                            parametersNode.AppendChild(tableNode);
                            break;
                        default:
                            XmlNode valueNode = body.CreateElement(param.Name.ToUpper());
                            valueNode.InnerText = this.structureMapper.ToRemoteValue(param.DataType, parameter.Value).ToString();
                            //if (!string.IsNullOrEmpty(valueNode.InnerText))
                                parametersNode.AppendChild(valueNode);
                            break;
                    }
                }
            }

            //table parameters are mandatory
            foreach (var param in this.function.InputParameters)
            {
                if (param.DataType == AbapDataType.TABLE)
                {
                    bool notAdded = this.parameters.TrueForAll(x => x.Name != param.Name);
                    if (notAdded)
                        parametersNode.AppendChild(body.CreateElement(param.Name));
                }
            }

            var responseXml = this.webClient.SendRfcRequest(this.function.Name, Beautify(body));

            var responseTag = responseXml.GetElementsByTagName(string.Format("urn:{0}.Response", this.function.Name));
            if (responseTag.Count > 0)
                return new SoapRfcResult(this.function, responseTag[0], this.structureMapper);

            var exceptionTag = responseXml.GetElementsByTagName(string.Format("rfc:{0}.Exception", this.function.Name));
            if (exceptionTag.Count > 0)
                throw new SharpRfcException(exceptionTag[0].InnerText);

            var faultErrorTag = responseXml.GetElementsByTagName("rfc:Error");
            if (faultErrorTag.Count > 0)
            {
                string errorText = faultErrorTag[0].SelectSingleNode("message").InnerText;
                string requestBody = body.InnerXml.ToString();
                string errorMessage = string.Format("Error: {0} Request Body: {1}", errorText, requestBody);
                throw new SharpRfcException(errorMessage);
            }

            var soapFaultTag = responseXml.GetElementsByTagName("SOAP-ENV:Fault");
            if (soapFaultTag.Count > 0)
            {
                string faultstring = soapFaultTag[0].SelectSingleNode("faultstring").InnerText;
                string detail = soapFaultTag[0].SelectSingleNode("detail").InnerText;
                string requestBody = body.InnerXml.ToString();
                string errorMessage = string.Format("Fault: {0} Detail: {1} Request Body: {2}", faultstring, detail, requestBody);
                throw new SharpRfcException(errorMessage);
            }

            throw new Exception("Could not fetch response tag.");
        }

        static public string Beautify(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }
    }
}
