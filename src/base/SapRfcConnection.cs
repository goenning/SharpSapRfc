using SharpSapRfc.Structure;
using System;
using System.Collections.Generic;

namespace SharpSapRfc
{
    public abstract class SapRfcConnection : IDisposable
    {
        public abstract RfcPreparedFunction PrepareFunction(string functionName);
        public abstract void Dispose();

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

        public abstract IEnumerable<T> ReadTable<T>(string tableName, string[] fields = null, string[] where = null, int skip = 0, int count = 0);
    }
}
