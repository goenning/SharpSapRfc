using SharpSapRfc.Metadata;
using SharpSapRfc.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SharpSapRfc.Soap
{
    public class SoapAbapMetadataCache : AbapMetadataCache
    {
        private SapSoapRfcWebClient webClient;
        private XmlNamespaceManager nsmgr;
        public SoapAbapMetadataCache(SapSoapRfcWebClient webClient)
        {
            this.webClient = webClient;
        }

        protected override FunctionMetadata LoadFunctionMetadata(string functionName)
        {
            var responseXml = this.webClient.SendWsdlRequest(functionName);

            List<ParameterMetadata> inputParameters = new List<ParameterMetadata>();
            List<ParameterMetadata> outputParameters = new List<ParameterMetadata>();

            this.nsmgr = new XmlNamespaceManager(responseXml.NameTable);
            this.nsmgr.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");

            var types = responseXml.GetElementsByTagName("types");
            if (types.Count > 0)
            {
                var inputElement = types[0].SelectSingleNode(string.Format("xsd:schema/xsd:element[@name='{0}']", functionName), nsmgr);
                if (inputElement != null)
                {
                    var nodes = inputElement.SelectNodes("xsd:complexType/xsd:all/xsd:element", this.nsmgr);
                    foreach (XmlNode node in nodes)
                        inputParameters.Add(this.ExtractParameterFromXmlNode(node));
                }

                var outputElement = types[0].SelectSingleNode(string.Format("xsd:schema/xsd:element[@name='{0}.Response']", functionName), nsmgr);
                if (outputElement != null)
                {
                    var nodes = outputElement.SelectNodes("xsd:complexType/xsd:all/xsd:element", this.nsmgr);
                    foreach (XmlNode node in nodes)
                        outputParameters.Add(this.ExtractParameterFromXmlNode(node));
                }
            }
            return new FunctionMetadata(functionName, inputParameters, outputParameters);
        }

        private ParameterMetadata ExtractParameterFromXmlNode(XmlNode node)
        {
            string name = node.Attributes["name"].Value;
            if (node.Attributes["type"] != null)
            {
                AbapDataType type = AbapDataTypeParser.ParseFromTypeAttribute(node.Attributes["type"].Value, false);
                return new ParameterMetadata(name, type);
            }
            else
            {
                var simpleType = node.SelectSingleNode("xsd:simpleType/xsd:restriction", nsmgr);
                if (simpleType != null) 
                {
                    AbapDataType type = AbapDataTypeParser.ParseFromTypeAttribute(simpleType.Attributes["base"].Value, false);
                    return new ParameterMetadata(name, type);
                }
                else
                {
                    var sequenceType = node.SelectSingleNode("xsd:complexType/xsd:sequence/xsd:element/xsd:simpleType/xsd:restriction", nsmgr);
                    AbapDataType type = AbapDataTypeParser.ParseFromTypeAttribute(sequenceType.Attributes["base"].Value, true);
                    return new ParameterMetadata(name, type);
                }

            }
            throw new RfcException("Erro when trying to parse XmlNode to Parameter Metadata.");
        }

        protected override StructureMetadata LoadStructureMetadata(string structureName)
        {
            throw new NotImplementedException();
        }
    }
}
