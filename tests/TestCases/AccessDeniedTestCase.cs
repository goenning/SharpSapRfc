using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpSapRfc.Test.TestCases
{
    public class Soap_AccessDeniedTestCase : AccessDeniedTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new SoapSapRfcConnection("TST_DENY_ACCESS-SOAP");
        }
    }

    public class Plain_AccessDeniedTestCase : AccessDeniedTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new PlainSapRfcConnection("TST_DENY_ACCESS");
        }
    }

    public abstract class AccessDeniedTestCase
    {
        protected abstract SapRfcConnection GetConnection();

        [Fact]
        public void ShouldNotExecuteFunctionBecauseItIsNotAllowed()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                Assert.Throws<SharpRfcCallException>(() => {
                    var result = conn.ExecuteFunction("Z_SSRT_SUM",
                        new RfcParameter("i_num1", 2),
                        new RfcParameter("i_num2", 4)
                    );
                });
            }
        }
    }
}
