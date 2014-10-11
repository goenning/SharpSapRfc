using SAP.Middleware.Connector;
using SharpSapRfc.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Plain
{
    public class PlainAbapMetadataCache : AbapMetadataCache
    {
        private SapPlainRfcConnection connection;
        public PlainAbapMetadataCache(SapPlainRfcConnection connection)
        {
            this.connection = connection;
        }

        protected override FunctionMetadata LoadFunctionMetadata(string functionName)
        {
            var rfcMetadata = this.connection.Repository.GetFunctionMetadata(functionName);

            List<ParameterMetadata> importParameters = new List<ParameterMetadata>();
            List<ParameterMetadata> exportParameters = new List<ParameterMetadata>();

            for (int i = 0; i < rfcMetadata.ParameterCount; i++)
            {
                var parameter = new ParameterMetadata(rfcMetadata[i].Name, rfcMetadata[i].GetAbapDataType());
                switch (rfcMetadata[i].Direction)
                {
                    case RfcDirection.EXPORT:
                        exportParameters.Add(parameter);
                        break;
                    case RfcDirection.IMPORT:
                        importParameters.Add(parameter);
                        break;
                    case RfcDirection.TABLES:
                    case RfcDirection.CHANGING:
                    default:
                        break;
                }
            }

            FunctionMetadata metadata = new FunctionMetadata(functionName, importParameters, exportParameters);
            return metadata;
        }
    }
}
