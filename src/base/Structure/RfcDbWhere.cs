using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Structure
{
    public class RfcDbWhere
    {
        [RfcStructureField("TEXT")]
        public string Text { get; set; }

        public RfcDbWhere()
        {

        }

        public RfcDbWhere(string text)
        {
            this.Text = text;
        }
    }
}
