using SharpSapRfc.Types;

namespace SharpSapRfc.Metadata
{
    public class ParameterMetadata
    {
        public string Name { get; private set; }
        public AbapDataType DataType { get; private set; }
        public StructureMetadata StructureMetadata { get; private set; }

        public ParameterMetadata(string name, AbapDataType dataType)
        {
            this.Name = name.ToUpper();
            this.DataType = dataType;
        }

        public ParameterMetadata(string name, AbapDataType dataType, StructureMetadata structureMetadata)
        {
            this.Name = name.ToUpper();
            this.DataType = dataType;
            this.StructureMetadata = structureMetadata;
        }
    }
}
