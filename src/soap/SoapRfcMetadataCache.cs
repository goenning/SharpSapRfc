using SharpSapRfc.Metadata;
using SharpSapRfc.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SharpSapRfc.Soap
{
    public class SoapRfcMetadataCache : RfcMetadataCache
    {
        private XmlDocument responseXml;
        private SoapRfcWebClient webClient;
        private XmlNamespaceManager nsmgr;

        public SoapRfcMetadataCache(SoapRfcWebClient webClient)
        {
            this.webClient = webClient;
        }

        protected override FunctionMetadata LoadFunctionMetadata(string functionName)
        {
            this.responseXml = this.webClient.SendWsdlRequest(functionName);

            List<FieldMetadata> inputParameters = new List<FieldMetadata>();
            List<FieldMetadata> outputParameters = new List<FieldMetadata>();

            this.nsmgr = new XmlNamespaceManager(this.responseXml.NameTable);
            this.nsmgr.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");

            var types = this.responseXml.GetElementsByTagName("types");
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

        private FieldMetadata ExtractParameterFromXmlNode(XmlNode node)
        {
            string parameterName = node.Attributes["name"].Value;
            if (node.Attributes["type"] != null)
                return CreateParameterMetadata(parameterName, node.Attributes["type"].Value, false);

            var simpleType = node.SelectSingleNode("xsd:simpleType/xsd:restriction", nsmgr);
            if (simpleType != null) 
                return CreateParameterMetadata(parameterName, simpleType.Attributes["base"].Value, false);
             
            var sequenceElement = node.SelectSingleNode("xsd:complexType/xsd:sequence/xsd:element", nsmgr);
            if (sequenceElement.Attributes["type"] != null)
                return CreateParameterMetadata(parameterName, sequenceElement.Attributes["type"].Value, true);
             
            var sequenceSimpleType = sequenceElement.SelectSingleNode("xsd:simpleType/xsd:restriction", nsmgr);
                return CreateParameterMetadata(parameterName, sequenceSimpleType.Attributes["base"].Value, true);

            throw new SharpRfcException("Error when trying to parse XmlNode to ParameterMetadata.");
        }

        private FieldMetadata CreateParameterMetadata(string name, string typeName, bool isSequence)
        {
            AbapDataType parameterType = AbapDataTypeParser.ParseFromTypeAttribute(typeName, isSequence);
            StructureMetadata metadata = null;
            if (typeName.StartsWith("s0:"))
                metadata = this.LoadStructureMetadata(typeName.Replace("s0:", ""));

            return new FieldMetadata(name, parameterType, metadata);
        }

        protected override StructureMetadata LoadStructureMetadata(string structureName)
        {
            string xpath = string.Format("//xsd:complexType[@name='{0}']/xsd:sequence/xsd:element", structureName);
            var nodes = this.responseXml.SelectNodes(xpath, this.nsmgr);

            List<FieldMetadata> fields = new List<FieldMetadata>();
            foreach(XmlNode node in nodes)
            {
                FieldMetadata param = this.ExtractParameterFromXmlNode(node);
                fields.Add(new FieldMetadata(param.Name, param.DataType));
            }

            return new StructureMetadata(structureName, fields);
        }
    }
}
