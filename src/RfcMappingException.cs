using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc
{
    public class RfcMappingException : RfcBaseException
    {
        public RfcMappingException(string message)
            : base(message)
        {

        }
    }
}
