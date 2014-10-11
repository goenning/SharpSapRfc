using SharpSapRfc.Types;

namespace SharpSapRfc.Metadata
{
    public class FieldMetadata
    {
        public string FieldName { get; private set; }
        public AbapDataType DataType { get; private set; }

        public FieldMetadata(string fieldName, AbapDataType dataType)
        {
            this.FieldName = fieldName;
            this.DataType = dataType;
        }
    }
}
