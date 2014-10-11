using System;
using System.Collections.Generic;

namespace SharpSapRfc.Soap
{
    public class SapSoapRfcConnection : SapRfcConnection
    {
        public override RfcPreparedFunction PrepareFunction(string functionName)
        {
            return new SoapRfcPreparedFunction(functionName);
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
