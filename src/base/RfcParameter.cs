
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
            return string.Format("{0}: {1}", this.Name.ToUpper(), FormatValue(this.Value));
        }

        private bool IsCollection(Type type)
        {
            return (!typeof(String).Equals(type) &&
                typeof(IEnumerable).IsAssignableFrom(type));
        }

        private string FormatValue(object value) 
        {
            if (value == null) 
                return "null";

            Type type = value.GetType();
            if (type.IsPrimitive || type.Equals(typeof(string)))
                return value.ToString();
            else if (type.IsArray || IsCollection(type))
            {
                IEnumerator enumerator = (value as IEnumerable).GetEnumerator();

                StringBuilder output = new StringBuilder();
                output.Append("[ ");
                while (enumerator.MoveNext())
                {
                    output.AppendFormat("{0}, ", FormatValue(enumerator.Current));
                }
                output.Remove(output.Length - 2, 2);
                output.Append(" ]");
                return output.ToString(); 
            }
            else
            {
                StringBuilder output = new StringBuilder();
                output.Append("{ ");
                PropertyInfo[] properties = type.GetProperties();
                foreach (var prop in properties)
                {
                    string name = prop.Name;
                    RfcStructureFieldAttribute[] attributes = prop.GetCustomAttributes(typeof(RfcStructureFieldAttribute), false) as RfcStructureFieldAttribute[];
                    if (attributes != null && attributes.Length > 0)
                        name = attributes[0].FieldName;
                    output.AppendFormat("{0}: {1}, ", name.ToUpper(), this.FormatValue(prop.GetValue(value, null)));
                }
                output.Remove(output.Length - 2, 2);
                output.Append(" }");
                return output.ToString();
            }
        }
    }
}
