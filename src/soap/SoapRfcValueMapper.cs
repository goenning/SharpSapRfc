using System.Globalization;

namespace SharpSapRfc.Soap
{
    public class SoapRfcValueMapper : RfcValueMapper
    {
        protected override NumberFormatInfo GetNumberFormat()
        {
            return this.PeriodDecimalNumberFormat;
        }
    }
}
