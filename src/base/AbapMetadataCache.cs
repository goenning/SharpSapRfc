using SharpSapRfc.Metadata;
using System.Collections.Generic;

namespace SharpSapRfc
{
    public abstract class AbapMetadataCache
    {
        private static Dictionary<string, FunctionMetadata> _functionMetadataCache;
        private static object _syncObject = new object();

        public AbapMetadataCache()
        {
            _functionMetadataCache = new Dictionary<string, FunctionMetadata>();
        }

        public FunctionMetadata GetFunctionMetadata(string functionName) 
        {
            if (_functionMetadataCache.ContainsKey(functionName))
                return _functionMetadataCache[functionName];

            lock (_syncObject)
            {
                if (_functionMetadataCache.ContainsKey(functionName))
                    return _functionMetadataCache[functionName];

                var functionMetadata = this.LoadFunctionMetadata(functionName);
                _functionMetadataCache.Add(functionName, functionMetadata);
                return functionMetadata;
            }
        }

        protected abstract FunctionMetadata LoadFunctionMetadata(string functionName);
    }
}
