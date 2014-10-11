using System;
using System.Collections.Generic;
using System.Xml;

namespace SharpSapRfc.Soap
{
    public class SoapRfcResult : RfcResult
    {
        private XmlNode responseXml;
        public SoapRfcResult(XmlNode responseXml)
        {
            this.responseXml = responseXml;
        }

        public override T GetOutput<T>(string name)
        {
            /*XmlNode node = this.responseXml.SelectSingleNode(string.Format("//{0}", name.ToUpper()));
            if (node != null)
            {
                if (node.HasChildNodes && node.FirstChild.HasChildNodes)
                    return RfcStructureMapper.FromXml<T>(node);

                return (T)RfcValueMapper.FromRemoteValue(typeof(T), node.InnerText);
            }*/
            throw new RfcMappingException(string.Format("Could not find tag '{0}'", name));
        }

        public override IEnumerable<T> GetTable<T>(string name)
        {
            throw new NotImplementedException();
        }
    }
}
