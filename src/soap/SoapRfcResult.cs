using SharpSapRfc.Metadata;
using System;
using System.Collections.Generic;
using System.Xml;

namespace SharpSapRfc.Soap
{
    public class SoapRfcResult : RfcResult
    {
        private FunctionMetadata metadata;
        private XmlNode responseXml;
        public SoapRfcResult(FunctionMetadata metadata, XmlNode responseXml)
        {
            this.metadata = metadata;
            this.responseXml = responseXml;
        }

        public override T GetOutput<T>(string name)
        {
            var parameter = this.metadata.GetOutputParameter(name);
            
            XmlNode node = this.responseXml.SelectSingleNode(string.Format("//{0}", name.ToUpper()));
            if (node != null)
            {
                //if (node.HasChildNodes && node.FirstChild.HasChildNodes)
                    //return RfcStructureMapper.FromXml<T>(node);

                return (T)AbapValueMapper.FromRemoteValue(typeof(T), node.InnerText);
            }
            throw new RfcMappingException(string.Format("Could not find tag '{0}'", name));
        }

        public override IEnumerable<T> GetTable<T>(string name)
        {
            throw new NotImplementedException();
        }
    }
}
