using System;

namespace SharpSapRfc
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class RfcStructureFieldAttribute : Attribute
    {
        public string FieldName { get; private set; }
        public string SecondFieldName { get; private set; }

        public RfcStructureFieldAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }

        public RfcStructureFieldAttribute(string dateFieldName, string timeFieldName)
        {
            this.FieldName = dateFieldName;
            this.SecondFieldName = timeFieldName;
        }
    }
}
