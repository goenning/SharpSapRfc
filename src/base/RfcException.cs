using System;

namespace SharpSapRfc
{
    public class RfcException : Exception
    {
        public RfcException(string message)
            : base(message)
        {

        }
    }
}
