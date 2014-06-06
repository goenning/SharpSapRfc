using Xunit;

namespace SharpSapRfc.Test
{
    public class PreparedFunctionTestCase
    {
        [Fact]
        public void PreparedFunctionWithTwoParametersTest()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
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
