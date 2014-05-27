using SAP.Middleware.Connector;
using SharpSapRfc.Structure;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpSapRfc
{
    public class SapRfcConnection : IDisposable
    {
        public RfcRepository Repository { get; private set; }
        public RfcDestination Destination { get; private set; }

        public SapRfcConnection(string destinationName)
        {
            this.Destination = RfcDestinationManager.GetDestination(destinationName);
            this.Repository = this.Destination.Repository;
        }

        public RfcResult ExecuteFunction(string functionName)
        {
            return this.PrepareFunction(functionName)
                       .Execute();
        }

        public RfcResult ExecuteFunction(string functionName, object parameters)
        {
            return this.PrepareFunction(functionName)
                       .AddParameter(parameters)
                       .Execute();
        }

        public RfcResult ExecuteFunction(string functionName, params RfcParameter[] parameters)
        {
            return this.PrepareFunction(functionName)
                       .AddParameter(parameters)
                       .Execute();
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

            return RfcStructureMapper.FromRfcReadTableToList<T>(
                result.GetTable<Tab512>("DATA"),
                result.GetTable<RfcDbField>("FIELDS")
            );
        }

        public void Dispose()
        {
            
        }

        public RfcPreparedFunction PrepareFunction(string functionName)
        {
            return new RfcPreparedFunction(functionName, this);
        }
    }
}
