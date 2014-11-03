using SharpSapRfc.Metadata;
using SharpSapRfc.Soap.Configuration;

namespace SharpSapRfc.Soap
{
    public class SoapSapRfcConnection : SapRfcConnection
    {
        public SapSoapRfcDestinationElement Destination { get; private set; }
        private SoapRfcWebClient _webClient;
        private SoapRfcMetadataCache metadataCache;

        private SoapRfcStructureMapper _structureMapper;

        public SoapSapRfcConnection(string name)
        {
            this.Destination = SapSoapRfcConfigurationSection.GetConfiguration(name);
            this._webClient = new SoapRfcWebClient(this.Destination);
            this.metadataCache = new SoapRfcMetadataCache(this._webClient);
            this._structureMapper = new SoapRfcStructureMapper(new SoapRfcValueMapper());
        }

        public override RfcPreparedFunction PrepareFunction(string functionName)
        {
            FunctionMetadata metadata = this.metadataCache.GetFunctionMetadata(functionName);
            return new SoapRfcPreparedFunction(metadata, this._structureMapper, this._webClient);
        }

        public override void Dispose()
        {

        }

        protected override RfcStructureMapper GetStructureMapper()
        {
            return this._structureMapper;
        }

        public override void SetTimeout(int timeout)
        {
            this.Destination.Timeout = timeout;
        }
    }
}
