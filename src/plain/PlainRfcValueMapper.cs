using System.Globalization;

namespace SharpSapRfc.Plain
{
    public class PlainRfcValueMapper : RfcValueMapper
    {
        protected override NumberFormatInfo GetNumberFormat()
        {
            return this.CommaDecimalNumberFormat;
        }
    }
}
