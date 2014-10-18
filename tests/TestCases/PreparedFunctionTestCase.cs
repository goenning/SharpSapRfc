using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using Xunit;

namespace SharpSapRfc.Test.TestCases
{
    public class Soap_PreparedFunctionTestCase  : PreparedFunctionTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new SoapSapRfcConnection("TST-SOAP");
        }
    }

    public class Plain_PreparedFunctionTestCase : PreparedFunctionTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new PlainSapRfcConnection("TST");
        }
    }

    public abstract class PreparedFunctionTestCase
    {
        protected abstract SapRfcConnection GetConnection();

        [Fact]
        public void PreparedFunctionWithTwoParametersTest()
        {
            using (SapRfcConnection conn = GetConnection())
            {
                var function = conn.PrepareFunction("Z_SSRT_SUM");
                function.AddParameter(new RfcParameter("i_num1", 2));
                function.AddParameter(new { i_num2 = 4 });
                var result = function.Execute();

                var total = result.GetOutput<int>("e_result");
                Assert.Equal(6, total);
            }
        }
    }
}
