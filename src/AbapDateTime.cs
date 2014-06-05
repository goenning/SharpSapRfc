using SAP.Middleware.Connector;
using System;
using System.Globalization;

namespace SharpSapRfc
{
    public class AbapDateTime
    {
        private static CultureInfo enUS = new CultureInfo("en-US");

        public static DateTime FromString(string value)
        {
            DateTime date;

            if (DateTime.TryParseExact(value, new string[] { "yyyy-MM-dd", "yyyyMMdd" }, enUS, DateTimeStyles.AssumeLocal, out date))
            {
                return date;
            }
            else if (DateTime.TryParseExact(value, new string[] { "HH:mm:ss", "HHmmss" }, enUS, DateTimeStyles.AssumeLocal, out date))
            {
                return DateTime.MinValue.Add(new TimeSpan(date.Hour, date.Minute, date.Second));
            }
            else if (value == "00000000" || value == "000000") // ABAP Date and Time initial value
            {
                return DateTime.MinValue;
            }
            else
            {
                string message = string.Format("{0} is not a valid Date format.", value);
                throw new RfcAbapException("UNKNOWN_DATETIME_FORMAT", message, message);
            }
        }

        public static string ToString(DateTime value, AbapDateTimeType type)
        {
            if (type == AbapDateTimeType.Date)
            {
                return value.ToString("yyyyMMdd");
            }
            else if (type == AbapDateTimeType.Time)
            {
                return value.ToString("HHmmss");
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
