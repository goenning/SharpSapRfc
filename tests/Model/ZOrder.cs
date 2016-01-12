using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Test.Model
{
    public class ZOrder
    {
        public int Id { get; set; }
        public IEnumerable<ZOrderLine> Items { get; set; }
    }
}
