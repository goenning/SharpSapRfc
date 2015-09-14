using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpSapRfc.Test.TestCases
{
    public class Soap_ExceptionDetailsTestCase : ExceptionDetailsTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new SoapSapRfcConnection("TST-SOAP");
        }

        protected override string GetExpectedRequestBody()
        {
            return @"<urn:Z_SSRT_DIVIDE xmlns:urn=""urn:sap-com:document:sap:rfc:functions""><I_NUM1>2</I_NUM1><I_NUM2>0</I_NUM2></urn:Z_SSRT_DIVIDE>";
        }
    }

    public class Plain_ExceptionDetailsTestCase : ExceptionDetailsTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new PlainSapRfcConnection("TST");
        }

        protected override string GetExpectedRequestBody()
        {
            return @"FUNCTION Z_SSRT_DIVIDE (EXPORT PARAMETER E_QUOTIENT=0.00, EXPORT PARAMETER E_REMAINDER=0, IMPORT PARAMETER I_NUM1=2.00, IMPORT PARAMETER I_NUM2=0.00)";
        }
    }

    public abstract class ExceptionDetailsTestCase
    {
        protected abstract SapRfcConnection GetConnection();
        protected abstract string GetExpectedRequestBody();

        [Fact]
        public void ShouldReturnRequestBodyOnExceptionTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {

                SharpRfcCallException ex = Assert.Throws<SharpRfcCallException>(() =>
                {
                    var result = conn.ExecuteFunction("Z_SSRT_DIVIDE", new
                    {
                        i_num1 = 2,
                        i_num2 = 0
                    });
                });

                Assert.Equal(this.GetExpectedRequestBody(), ex.RequestBody);
            }
        }
        [Fact]
        public void ExceptionToStringShouldReturnRequestBodyTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {

                SharpRfcCallException ex = Assert.Throws<SharpRfcCallException>(() =>
                {
                    var result = conn.ExecuteFunction("Z_SSRT_DIVIDE", new
                    {
                        i_num1 = 2,
                        i_num2 = 0
                    });
                });

                Assert.Contains(this.GetExpectedRequestBody(), ex.ToString());
            }
        }
    }
}
