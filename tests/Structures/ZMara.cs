using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Test.Structures
{
    public class ZMara
    {
        [RfcStructureField("ID")]
        public int Id { get; set; }
        [RfcStructureField("NAME")]
        public string Name { get; set; }
        [RfcStructureField("PRICE")]
        public decimal Price { get; set; }
        [RfcStructureField("DATUM")]
        public DateTime Date { get; set; }
        [RfcStructureField("UZEIT")]
        public DateTime Time { get; set; }
        [RfcStructureField("Active")]
        public bool IsActive { get; set; }
    }
}
