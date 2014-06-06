using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SharpSapRfc
{
    public class RfcValueMapper
    {
        private static CultureInfo enUS = new CultureInfo("en-US");
        private static CultureInfo ptBR = new CultureInfo("pt-BR");

        public static object FromRemoteValue(Type type, object value)
        {
            object returnValue = null;
            if (type.Equals(typeof(Boolean)))
                returnValue = AbapBool.FromString(value.ToString());
            else if (type.Equals(typeof(DateTime)))
                returnValue = AbapDateTime.FromString(value.ToString());
            else if (type.Equals(typeof(Int32)))
                returnValue = Convert.ToInt32(value);
            else if (type.Equals(typeof(Decimal)))
                returnValue = Convert.ToDecimal(value, value.ToString().Contains(",") ? ptBR : enUS);
            else if (type.Equals(typeof(Double)))
                returnValue = Convert.ToDouble(value, value.ToString().Contains(",") ? ptBR : enUS);
            else
                returnValue = Convert.ChangeType(value, type);

            return returnValue;
        }

        public static object ToRemoteValue(RfcDataType remoteType, object value)
        {
            if (value == null)
                return null;

            Type valueType = value.GetType();
            object returnValue = value;

            if (valueType.Equals(typeof(Boolean)))
                returnValue = AbapBool.ToString((Boolean)value);
            else if (remoteType == RfcDataType.DATE)
                returnValue = AbapDateTime.ToString((DateTime)value, AbapDateTimeType.Date);
            else if (remoteType == RfcDataType.TIME)
                returnValue = AbapDateTime.ToString((DateTime)value, AbapDateTimeType.Time);

            return returnValue;
        }
    }
}
