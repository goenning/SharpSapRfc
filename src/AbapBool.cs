using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpSapRfc
{
    public static class AbapBool
    {
        private static List<string> expectedValues = new List<string>() { "X", "", " " };

        public static Boolean FromString(string value)
        {
            if (expectedValues.Contains(value))
                return value == "X";

            string message = string.Format("'{0}' is not a valid boolean value", value);
            throw new RfcMappingException(message);
        }

        public static string ToString(bool value)
        {
            return value ? "X" : " ";
        }
    }
}
