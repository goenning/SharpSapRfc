using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc
{
    public class SharpRfcCallException : SharpRfcException
    {
        public string RequestBody { get; private set; }

        public SharpRfcCallException(string message, string requestBody)
            : this(message, requestBody, null)
        {
        }

        public SharpRfcCallException(string message, Exception innerException)
            : this(message, null, innerException)
        {
        }

        public SharpRfcCallException(string message, string requestBody, Exception innerException)
            : base(message, innerException)
        {
            this.RequestBody = requestBody;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.RequestBody))
                return base.ToString();

            return string.Format("{0} Request Body was: {1}", base.ToString(), this.RequestBody);
        }
    }
}
