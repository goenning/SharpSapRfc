using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpSapRfc.Test.TestCases
{
    public class Soap_TimeoutTestCase : TimeoutTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new SoapSapRfcConnection("TST-SOAP");
        }

        [Fact]
        public void ShouldThrowTimeoutException()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                Exception ex = Assert.Throws<SharpRfcCallException>(() =>
                {
                    var result = conn.ExecuteFunction("Z_SSRT_LONG_PROCESS", new
                    {
                        i_seconds = 6
                    });
                });

                Assert.IsType<TimeoutException>(ex.InnerException);
            }
        }
    }

    public class Plain_TimeoutTestCase : TimeoutTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new PlainSapRfcConnection("TST");
        }
    }

    public abstract class TimeoutTestCase
    {
        protected abstract SapRfcConnection GetConnection();

        [Fact]
        public void ShouldNotThrowTimeoutException()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_LONG_PROCESS", new
                {
                    i_seconds = 1
                });
            }
        }
    }
}
