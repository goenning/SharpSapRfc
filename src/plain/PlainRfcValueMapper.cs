using System.Globalization;
using System.Threading;

namespace SharpSapRfc.Plain
{
    public class PlainRfcValueMapper : RfcValueMapper
    {
        protected override NumberFormatInfo GetNumberFormat()
        {
            return Thread.CurrentThread.CurrentCulture.NumberFormat;
        }
    }
}
