using SAP.Middleware.Connector;
using SharpSapRfc.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Plain
{
    public class SapPlainRfcConnection : SapRfcConnection
    {
        public RfcRepository Repository { get; private set; }
        public RfcDestination Destination { get; private set; }
        private string destinationName;
        private bool isOpen = false;

        public SapPlainRfcConnection(string destinationName)
        {
            this.destinationName = destinationName;
        }

        private void EnsureConnectionIsOpen()
        {
            if (!isOpen)
            {
                this.Destination = RfcDestinationManager.GetDestination(destinationName);
                this.Repository = this.Destination.Repository;
                this.isOpen = true;
            }
        }

        public override RfcPreparedFunction PrepareFunction(string functionName)
        {
            EnsureConnectionIsOpen();
            return new PlainRfcPreparedFunction(functionName, this.Repository, this.Destination);
        }

        public override void Dispose()
        {
            
        }

        public override IEnumerable<T> ReadTable<T>(string tableName, string[] fields = null, string[] where = null, int skip = 0, int count = 0)
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

            return RfcStructureMapper.FromRfcReadTableToList<T>(
                result.GetTable<Tab512>("DATA"),
                result.GetTable<RfcDbField>("FIELDS")
            );
        }
    }
}
