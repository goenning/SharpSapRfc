using SAP.Middleware.Connector;
using System;

namespace SharpSapRfc
{
    public class SharpSapRfcConnection : IDisposable
    {
        private string connectionString;
        private RfcRepository repository;
        private RfcDestination destination;
        private RfcStructureMapper mapper;

        public SharpSapRfcConnection(string connectionString)
        {
            this.connectionString = connectionString;

            if (!RfcDestinationManager.IsDestinationConfigurationRegistered()) 
            {
                RfcDestinationConfig config = new RfcDestinationConfig();
                RfcDestinationManager.RegisterDestinationConfiguration(config);
            }

            this.destination = RfcDestinationManager.GetDestination("NSP");
            this.repository = destination.Repository;
            this.mapper = new RfcStructureMapper();
        }

        public void Dispose()
        {
        }

        public RfcResult ExecuteFunction(string functionName)
        {
            return this.ExecuteFunction(functionName, new RfcParameter[0]);
        }

        public RfcResult ExecuteFunction(string functionName, params RfcParameter[] importParameters)
        {
            IRfcFunction function = this.repository.CreateFunction(functionName);
            foreach (var parameter in importParameters)
            {
                int idx = function.Metadata.TryNameToIndex(parameter.Name);
                RfcDataType pType = function.Metadata[idx].DataType;
                switch (pType)
                {
                    case RfcDataType.STRUCTURE:
                        RfcStructureMetadata structureMetadata = function.GetStructure(idx).Metadata;
                        IRfcStructure structure = this.mapper.CreateStructure(structureMetadata, parameter.Value);
                        function.SetValue(parameter.Name, structure);
                        break;
                    case RfcDataType.TABLE:
                        RfcTableMetadata tableMetadata = function.GetTable(idx).Metadata;
                        IRfcTable table = this.mapper.CreateTable(tableMetadata, parameter.Value);
                        function.SetValue(parameter.Name, table);
                        break;
                    default:
                        function.SetValue(parameter.Name, parameter.Value);
                        break;
                }
            }

            function.Invoke(destination);
            return new RfcResult(function, this.mapper);
        }
    }
}
