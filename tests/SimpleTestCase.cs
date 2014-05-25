using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAP.Middleware.Connector;

namespace SharpSapRfc.Test
{
    [TestClass]
    public class SimpleTestCase
    {
        [TestMethod]
        public void ExportSingleParameterTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
            {
                var result = conn.ExecuteFunction("Z_SSRT_SUM", 
                    new RfcParameter("i_num1", 2),
                    new RfcParameter("i_num2", 4)
                );

                var total = result.GetOutput<int>("e_result");
                Assert.AreEqual(6, total);
            }
        }

        [TestMethod]
        public void ExportSingleParameterTest_WithAnonymousType()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
            {
                var result = conn.ExecuteFunction("Z_SSRT_SUM", new
                {
                    i_num1 = 2,
                    i_num2 = 7
                });

                var total = result.GetOutput<int>("e_result");
                Assert.AreEqual(9, total);
            }
        }

        [TestMethod]
        public void ChangingSingleParameterTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
            {
                var result = conn.ExecuteFunction("Z_SSRT_ADD",
                    new RfcParameter("i_add", 4),
                    new RfcParameter("c_num", 4)
                );

                var total = result.GetOutput<int>("c_num");
                Assert.AreEqual(8, total);
            }
        }

        [TestMethod]
        public void ExportMultipleParametersTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
            {
                var result = conn.ExecuteFunction("Z_SSRT_DIVIDE", 
                    new RfcParameter("i_num1", 5),
                    new RfcParameter("i_num2", 2)
                );

                var quotient = result.GetOutput<decimal>("e_quotient");
                var remainder = result.GetOutput<int>("e_remainder");
                Assert.AreEqual(2.5m, quotient);
                Assert.AreEqual(1, remainder);
            }
        }

        [TestMethod]
        public void ExceptionTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
            {
                try
                {
                    var result = conn.ExecuteFunction("Z_SSRT_DIVIDE", 
                        new RfcParameter("i_num1", 5),
                        new RfcParameter("i_num2", 0)
                    );
                }
                catch (RfcAbapException ex)
                {
                    Assert.AreEqual("DIVIDE_BY_ZERO", ex.Key);
                }
            }
        }
    }
}
