using SharpSapRfc.Types;

namespace SharpSapRfc.Soap
{
    public static class AbapDataTypeParser
    {
        public static AbapDataType ParseFromTypeAttribute(string type, bool isSequence)
        {
            if (isSequence)
                return AbapDataType.TABLE;

            switch (type)
            {
                case "xsd:base64Binary":
                    return AbapDataType.BYTE;

                case "s0:date":
                    return AbapDataType.DATE;

                case "s0:time":
                    return AbapDataType.TIME;

                case "xsd:decimal":
                    return AbapDataType.DECIMAL;

                case "xsd:string":
                    return AbapDataType.CHAR;

                case "xsd:int":
                    return AbapDataType.INTEGER;
            }

            if (type.StartsWith("s0:"))
                return AbapDataType.STRUCTURE;

            throw new RfcException(string.Format("Could not map xml type attribute '{0}' to AbapDataType.", type));
        }
    }
}
