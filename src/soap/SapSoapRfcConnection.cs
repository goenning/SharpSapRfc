using SharpSapRfc.Metadata;
using SharpSapRfc.Soap.Configuration;
using System;
using System.Collections.Generic;
using System.Xml;

namespace SharpSapRfc.Soap
{
    public class SapSoapRfcConnection : SapRfcConnection
    {
        public SapSoapRfcDestinationElement Destination { get; private set; }
        public SapSoapRfcWebClient WebClient { get; private set; }
        private SoapAbapMetadataCache metadataCache;

        public SapSoapRfcConnection(string name)
        {
            this.Destination = SapSoapRfcConfigurationSection.GetConfiguration(name);
            this.WebClient = new SapSoapRfcWebClient(this.Destination);
            this.metadataCache = new SoapAbapMetadataCache(this.WebClient);
        }

        public override RfcPreparedFunction PrepareFunction(string functionName)
        {
            FunctionMetadata metadata = this.metadataCache.GetFunctionMetadata(functionName);
            return new SoapRfcPreparedFunction(metadata, this.WebClient);
        }

        public override void Dispose()
        {

        }

        public override IEnumerable<T> ReadTable<T>(string tableName, string[] fields = null, string[] where = null, int skip = 0, int count = 0)
        {
            throw new NotImplementedException();
        }
    }
}
