using SAP.Middleware.Connector;
using SharpSapRfc.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Plain
{
    public static class RfcElementMetadataExtension
    {
        public static AbapDataType GetAbapDataType(this RfcElementMetadata metadata)
        {
            switch (metadata.DataType)
            {
                case RfcDataType.CHAR:
                case RfcDataType.STRING:
                    return AbapDataType.CHAR;

                case RfcDataType.DATE:
                    return AbapDataType.DATE;

                case RfcDataType.BCD:
                case RfcDataType.DECF16:
                case RfcDataType.DECF34:
                case RfcDataType.FLOAT:
                    return AbapDataType.DECIMAL;

                case RfcDataType.INT1:
                case RfcDataType.INT2:
                case RfcDataType.INT4:
                case RfcDataType.INT8:
                    return AbapDataType.INTEGER;

                case RfcDataType.NUM:
                    return AbapDataType.NUMERIC;

                case RfcDataType.STRUCTURE:
                    return AbapDataType.STRUCTURE;

                case RfcDataType.TABLE:
                    return AbapDataType.TABLE;

                case RfcDataType.TIME:
                    return AbapDataType.TIME;

                case RfcDataType.BYTE:
                case RfcDataType.XSTRING:
                    return AbapDataType.BYTE;

                case RfcDataType.CDAY:
                case RfcDataType.CLASS:
                case RfcDataType.DTDAY:
                case RfcDataType.DTMONTH:
                case RfcDataType.DTWEEK:
                case RfcDataType.TMINUTE:
                case RfcDataType.TSECOND:
                case RfcDataType.UNKNOWN:
                case RfcDataType.UTCLONG:
                case RfcDataType.UTCMINUTE:
                case RfcDataType.UTCSECOND:
                default:
                    throw new RfcException(string.Format("Could not map RfcDataType '{0}' to AbapDataType", metadata.DataType.ToString()));

            }
        }
    }
}
