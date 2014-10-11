using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Metadata
{
    public class FunctionMetadata
    {
        public string FunctionName { get; private set; }
        public ParameterMetadata[] ImportParameters { get; private set; }
        public ParameterMetadata[] ExportParameters { get; private set; }

        public FunctionMetadata(string functionName, IEnumerable<ParameterMetadata> importParameters, IEnumerable<ParameterMetadata> emportParameters)
        {
            this.FunctionName = functionName;
            this.ImportParameters = importParameters.ToArray();
            this.ExportParameters = emportParameters.ToArray();
        }

        public ParameterMetadata GetImportParameter(int index) 
        {
            return this.ImportParameters[index];
        }

        public ParameterMetadata GetExportParameter(int index)
        {
            return this.ExportParameters[index];
        }
    }
}
