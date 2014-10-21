using System.Collections.Generic;
using System.Linq;

namespace SharpSapRfc.Metadata
{
    public class FunctionMetadata
    {
        public string Name { get; private set; }
        public ParameterMetadata[] InputParameters { get; private set; }
        public ParameterMetadata[] OutputParameters { get; private set; }

        public FunctionMetadata(string name, IEnumerable<ParameterMetadata> inputParameters, IEnumerable<ParameterMetadata> outputParameters)
        {
            this.Name = name;
            this.InputParameters = inputParameters.ToArray();
            this.OutputParameters = outputParameters.ToArray();
        }

        public ParameterMetadata GetInputParameter(int index) 
        {
            return this.InputParameters[index];
        }

        public ParameterMetadata GetInputParameter(string name)
        {
            var parameter = this.InputParameters.FirstOrDefault(x => x.Name == name.ToUpper());
            if (parameter == null)
                throw new UnknownRfcParameterException(name, this.Name);
            return parameter;
        }

        public ParameterMetadata GetOutputParameter(int index)
        {
            return this.OutputParameters[index];
        }

        public ParameterMetadata GetOutputParameter(string name)
        {
            var parameter = this.OutputParameters.FirstOrDefault(x => x.Name == name.ToUpper());
            if (parameter == null)
                throw new UnknownRfcParameterException(name, this.Name);
            return parameter;
        }
    }
}
