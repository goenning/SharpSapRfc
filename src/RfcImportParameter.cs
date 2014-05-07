
namespace SharpSapRfc
{
    public class RfcImportParameter
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public RfcImportParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
