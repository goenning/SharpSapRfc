using System.Collections.Generic;

namespace SharpSapRfc
{
    public abstract class RfcResult
    {
        public abstract T GetOutput<T>(string name);
        public abstract IEnumerable<T> GetTable<T>(string name);
    }
}
