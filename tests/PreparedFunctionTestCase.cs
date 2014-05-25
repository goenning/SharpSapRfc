using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpSapRfc.Test
{
    [TestClass]
    public class PreparedFunctionTestCase
    {
        [TestMethod]
        public void PreparedFunctionWithTwoParametersTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
            {
                var function = conn.PrepareFunction("Z_SSRT_SUM");
                function.AddParameter(new RfcParameter("i_num1", 2));
                function.AddParameter(new { i_num2 = 4 });
                var result = function.Execute();

                var total = result.GetOutput<int>("e_result");
                Assert.AreEqual(6, total);
            }
        }
    }
}
