using System.Collections.Generic;
using System.Linq;

namespace SharpSapRfc.Metadata
{
    public class FunctionMetadata
    {
        public string Name { get; private set; }
        public FieldMetadata[] InputParameters { get; private set; }
        public FieldMetadata[] OutputParameters { get; private set; }

        public FunctionMetadata(string name, IEnumerable<FieldMetadata> inputParameters, IEnumerable<FieldMetadata> outputParameters)
        {
            this.Name = name;
            this.InputParameters = inputParameters.ToArray();
            this.OutputParameters = outputParameters.ToArray();
        }

        public FieldMetadata GetInputParameter(int index) 
        {
            return this.InputParameters[index];
        }

        public FieldMetadata GetInputParameter(string name)
        {
            var parameter = this.InputParameters.FirstOrDefault(x => x.Name == name.ToUpper());
            if (parameter == null)
                throw new UnknownRfcParameterException(name, this.Name);
            return parameter;
        }

        public FieldMetadata GetOutputParameter(int index)
        {
            return this.OutputParameters[index];
        }

        public FieldMetadata GetOutputParameter(string name)
        {
            var parameter = this.OutputParameters.FirstOrDefault(x => x.Name == name.ToUpper());
            if (parameter == null)
                throw new UnknownRfcParameterException(name, this.Name);
            return parameter;
        }
    }
}
