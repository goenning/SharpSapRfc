using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class RfcEnumValueAttribute : Attribute
    {
        public string Value { get; private set; }
        public RfcEnumValueAttribute(string value)
        {
            this.Value = value;
        }
    }
}
