using System;

namespace SharpSapRfc
{
    [Serializable]
    public class SharpRfcException : Exception
    {
        public SharpRfcException(string message)
            : base(message)
        {

        }

        public SharpRfcException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
