
using System;
using System.Collections;
using System.Reflection;
using System.Text;
namespace SharpSapRfc
{
    public class RfcParameter
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public RfcParameter(string name, object value)
        {
            this.Name = name.ToUpper();
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Name, this.Value);
        }
    }
}
