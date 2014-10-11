using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Metadata
{
    public class FunctionMetadata
    {
        public string Name { get; private set; }
        public IEnumerable<ParameterMetadata> Parameters { get; private set; }

        public FunctionMetadata(string name, IEnumerable<ParameterMetadata> fields)
        {
            this.Name = name;
            this.Parameters = fields;
        }

        public ParameterMetadata this[int index] 
        {
            get { return this.Parameters.ElementAt(index); }
        }
    }
}
