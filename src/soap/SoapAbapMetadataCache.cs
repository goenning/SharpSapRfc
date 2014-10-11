using SharpSapRfc.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Soap
{
    public class SoapAbapMetadataCache : AbapMetadataCache
    {
        protected override FunctionMetadata LoadFunctionMetadata(string functionName)
        {
            throw new NotImplementedException();
        }
    }
}
