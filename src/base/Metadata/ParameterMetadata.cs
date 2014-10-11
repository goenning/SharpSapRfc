using SharpSapRfc.Types;

namespace SharpSapRfc.Metadata
{
    public class ParameterMetadata
    {
        public string Name { get; private set; }
        public AbapDataType DataType { get; private set; }

        public ParameterMetadata(string name, AbapDataType dataType)
        {
            this.Name = name;
            this.DataType = dataType;
        }
    }
}
