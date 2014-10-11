using System.Collections.Generic;
using System.Linq;

namespace SharpSapRfc.Metadata
{
    public class StructureMetadata
    {
        public string Name { get; private set; }
        public IEnumerable<FieldMetadata> Fields { get; private set; }

        public StructureMetadata(string name, IEnumerable<FieldMetadata> fields)
        {
            this.Name = name;
            this.Fields = fields;
        }

        public FieldMetadata this[int index]
        {
            get { return this.Fields.ElementAt(index); }
        }
    }
}
