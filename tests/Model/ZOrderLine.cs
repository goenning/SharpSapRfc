using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Test.Model
{
    public class ZOrderLine
    {
        public ZMara Product { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
