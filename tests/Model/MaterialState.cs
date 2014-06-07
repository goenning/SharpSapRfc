using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Test.Model
{
    public enum MaterialState
    {
        [RfcEnumValue("AVAL")]
        Available = 1,
        [RfcEnumValue("BLOK")]
        Blocked = 2,
        [RfcEnumValue("OOS")]
        OutOfStock = 3,
    }
}
