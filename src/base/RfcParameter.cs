
namespace SharpSapRfc
{
    public class RfcParameter
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public RfcParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format("Name: {0} (Value: {1})", this.Name, this.Value);           
        }
    }
}
