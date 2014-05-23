using SAP.Middleware.Connector;
using SharpSapRfc.Structure;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpSapRfc
{
    public class SharpSapRfcConnection : IDisposable
    {
        private RfcStructureMapper mapper;
        private string connectionString;
        public RfcRepository Repository { get; private set; }
        public RfcDestination Destination { get; private set; }

        public SharpSapRfcConnection(string connectionString)
        {
            this.connectionString = connectionString;

            if (!RfcDestinationManager.IsDestinationConfigurationRegistered()) 
            {
                RfcDestinationConfig config = new RfcDestinationConfig();
                RfcDestinationManager.RegisterDestinationConfiguration(config);
            }

            this.Destination = RfcDestinationManager.GetDestination("NSP");
            this.Repository = this.Destination.Repository;
            this.mapper = new RfcStructureMapper();
        }

        public RfcResult ExecuteFunction(string functionName)
        {
            return this.ExecuteFunction(functionName, new RfcParameter[0]);
        }

        public RfcResult ExecuteFunction(string functionName, object parameters)
        {
            Type t = parameters.GetType();
            PropertyInfo[] properties = t.GetProperties();
            RfcParameter[] rfcParameters = new RfcParameter[properties.Length];
            for (int i = 0; i < properties.Length; i++)
                rfcParameters[i] = new RfcParameter(properties[i].Name, properties[i].GetValue(parameters, null));

            return this.ExecuteFunction(functionName, rfcParameters);
        }

        public RfcResult ExecuteFunction(string functionName, params RfcParameter[] parameters)
        {
            IRfcFunction function = this.Repository.CreateFunction(functionName);
            foreach (var parameter in parameters)
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

            function.Invoke(this.Destination);
            return new RfcResult(function, this.mapper);
        }

        public IEnumerable<T> ReadTable<T>(string tableName, string[] fields = null, string[] where = null, int skip = 0, int count = 0)
        {
            fields = fields ?? new string[0];
            where = where ?? new string[0];

            List<RfcDbField> dbFields = new List<RfcDbField>();
            for (int i = 0; i < fields.Length; i++)
                dbFields.Add(new RfcDbField(fields[i]));

            List<RfcDbWhere> dbWhere = new List<RfcDbWhere>();
            for (int i = 0; i < where.Length; i++)
                dbWhere.Add(new RfcDbWhere(where[i]));

            var result = this.ExecuteFunction("RFC_READ_TABLE", new {
                Query_Table = tableName,
                Fields = dbFields,
                Options = dbWhere,
                RowSkips = skip,
                RowCount = count
            });

            return this.mapper.FromRfcReadTableToList<T>(
                result.GetTable<Tab512>("DATA"),
                result.GetTable<RfcDbField>("FIELDS")
            );
        }

        public void Dispose()
        {

        }
    }
}
