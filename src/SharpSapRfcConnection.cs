using SAP.Middleware.Connector;
using SharpSapRfc.Structure;
using System;
using System.Collections.Generic;

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

        public IEnumerable<T> ReadTable<T>(string tableName)
        {
            return this.ReadTable<T>(tableName, new string[0]);
        }

        public IEnumerable<T> ReadTable<T>(string tableName, string[] fields)
        {
            List<RfcDbField> dbFields = new List<RfcDbField>();
            for (int i = 0; i < fields.Length; i++)
                dbFields.Add(new RfcDbField(fields[i]));

            var result = this.ExecuteFunction("RFC_READ_TABLE",
                new RfcParameter("QUERY_TABLE", tableName),
                new RfcParameter("FIELDS", dbFields)
            );

            return this.mapper.FromRfcReadTableToList<T>(
                result.GetTable<Tab512>("DATA"), 
                result.GetTable<RfcDbField>("FIELDS")
            );
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
