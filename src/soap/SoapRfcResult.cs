using SharpSapRfc.Metadata;
using System.Collections.Generic;
using System.Xml;

namespace SharpSapRfc.Soap
{
    public class SoapRfcResult : RfcResult
    {
        private SoapRfcStructureMapper structureMapper;
        private FunctionMetadata metadata;
        private XmlNode responseXml;

        public SoapRfcResult(FunctionMetadata metadata, XmlNode responseXml, SoapRfcStructureMapper structureMapper)
        {
            this.metadata = metadata;
            this.responseXml = responseXml;
            this.structureMapper = structureMapper;
        }

        public override T GetOutput<T>(string name)
        {
            var parameter = this.metadata.GetOutputParameter(name);
            
            XmlNode node = this.responseXml.SelectSingleNode(string.Format("//{0}", name.ToUpper()));
            if (node != null)
            {
                if (node.HasChildNodes && node.FirstChild.HasChildNodes)
                    return this.structureMapper.FromXml<T>(parameter.StructureMetadata, node);

                return (T)this.structureMapper.FromRemoteValue(typeof(T), node.InnerText);
            }
            throw new RfcMappingException(string.Format("Could not find tag '{0}'", name));
        }

        public override IEnumerable<T> GetTable<T>(string name)
        {
            var parameter = this.metadata.GetOutputParameter(name);

            XmlNodeList nodes = this.responseXml.SelectNodes(string.Format("//{0}/item", name.ToUpper()));
            List<T> returnTable = new List<T>(nodes.Count);
            for (int i = 0; i < nodes.Count; i++)
                returnTable.Add(this.structureMapper.FromXml<T>(parameter.StructureMetadata, nodes[i]));

            return returnTable;
        }
    }
}
