using SharpSapRfc.Metadata;
using System.Collections.Generic;

namespace SharpSapRfc
{
    public abstract class RfcMetadataCache
    {
        private static Dictionary<string, FunctionMetadata> _functionMetadataCache = new Dictionary<string, FunctionMetadata>();
        private static Dictionary<string, StructureMetadata> _structureMetadataCache = new Dictionary<string, StructureMetadata>();
        private static object _syncObject = new object();

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

        protected StructureMetadata GetStructureMetadata(string structureName)
        {
            if (_structureMetadataCache.ContainsKey(structureName))
                return _structureMetadataCache[structureName];

            lock (_syncObject)
            {
                if (_structureMetadataCache.ContainsKey(structureName))
                    return _structureMetadataCache[structureName];

                var structureMetadata = this.LoadStructureMetadata(structureName);
                _structureMetadataCache.Add(structureName, structureMetadata);
                return structureMetadata;
            }
        }

        protected abstract FunctionMetadata LoadFunctionMetadata(string functionName);
        protected abstract StructureMetadata LoadStructureMetadata(string structureName);
    }
}
