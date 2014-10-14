using System;

namespace SharpSapRfc
{
    public class RfcException : Exception
    {
        public RfcException(string message)
            : base(message)
        {

        }

        public RfcException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
