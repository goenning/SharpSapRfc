using SharpSapRfc.Structure;
using System;
using System.Collections.Generic;

namespace SharpSapRfc
{
    public abstract class SapRfcConnection : IDisposable
    {
        protected abstract RfcStructureMapper GetStructureMapper();
        public abstract RfcPreparedFunction PrepareFunction(string functionName);
        public abstract void Dispose();
        public abstract void SetTimeout(int timeout);

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

            var result = this.ExecuteFunction("RFC_READ_TABLE", new
            {
                Query_Table = tableName,
                Fields = dbFields,
                Options = dbWhere,
                RowSkips = skip,
                RowCount = count
            });

            return this.GetStructureMapper().FromRfcReadTableToList<T>(
                result.GetTable<Tab512>("DATA"),
                result.GetTable<RfcDbField>("FIELDS")
            );
        }

        public RfcReadTableQueryBuilder<T> Table<T>(string tableName)
        {
            return new RfcReadTableQueryBuilder<T>(this, tableName);
        }
    }
}
