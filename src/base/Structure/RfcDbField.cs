
namespace SharpSapRfc.Structure
{
    public class RfcDbField
    {
        [RfcStructureField("FIELDNAME")]
        public string FieldName { get; set; }

        [RfcStructureField("OFFSET")]
        public int Offset { get; set; }

        [RfcStructureField("LENGTH")]
        public int Length { get; set; }

        [RfcStructureField("TYPE")]
        public string Type { get; set; }

        [RfcStructureField("FIELDTEXT")]
        public string FieldText { get; set; }

        public RfcDbField()
        {
        }

        public RfcDbField(string fieldName)
        {
            this.FieldName = fieldName;
        }
    }
}
