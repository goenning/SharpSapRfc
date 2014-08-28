using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Test.Model
{
    public class ZMaraSingleDateTime
    {
        [RfcStructureField("ID")]
        public int Id { get; set; }
        [RfcStructureField("DATUM", "UZEIT")]
        public DateTime? DateTime { get; set; }
    }
}
