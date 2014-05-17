using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAP.Middleware.Connector;

namespace SharpSapRfc.Test
{
    [TestClass]
    public class SimpleTestCase
    {
        [TestMethod]
        public void SingleExportParametersTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                var result = conn.ExecuteFunction("Z_SSRT_SUM", new RfcImportParameter[] {
                    new RfcImportParameter("i_num1", 2),
                    new RfcImportParameter("i_num2", 4)
                });

                var total = result.GetExportParameter<int>("e_result");
                Assert.AreEqual(6, total);
            }
        }

        [TestMethod]
        public void MultipleExportParametersTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                var result = conn.ExecuteFunction("Z_SSRT_DIVIDE", new RfcImportParameter[] {
                    new RfcImportParameter("i_num1", 5),
                    new RfcImportParameter("i_num2", 2)
                });

                var quotient = result.GetExportParameter<decimal>("e_quotient");
                var remainder = result.GetExportParameter<int>("e_remainder");
                Assert.AreEqual(2.5m, quotient);
                Assert.AreEqual(1, remainder);
            }
        }

        [TestMethod]
        public void ExceptionTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                try
                {
                    var result = conn.ExecuteFunction("Z_SSRT_DIVIDE", new RfcImportParameter[] {
                        new RfcImportParameter("i_num1", 5),
                        new RfcImportParameter("i_num2", 0)
                    });
                }
                catch (RfcAbapException ex)
                {
                    Assert.AreEqual("DIVIDE_BY_ZERO", ex.Key);
                }
            }
        }

        [TestMethod]
        public void StructureExportTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                var result = conn.ExecuteFunction("Z_SSRT_GET_CUSTOMER", new RfcImportParameter[] {
                    new RfcImportParameter("i_id", 2)
                });

                var cliente = result.GetExportParameter<ZCustomer>("e_customer");
                Assert.AreEqual(2, cliente.Id);
                Assert.AreEqual("Walmart", cliente.Name);
                Assert.AreEqual(false, cliente.IsActive);
            }
        }

        [TestMethod]
        public void TableExportTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                var result = conn.ExecuteFunction("Z_SSRT_GET_ALL_CUSTOMERS");

                var clientes = result.GetExportTable<ZCustomer>("t_customers");
                Assert.AreEqual(2, clientes.Length);
                    
                Assert.AreEqual(1, clientes[0].Id);
                Assert.AreEqual("Apple Store", clientes[0].Name);
                Assert.AreEqual(0, clientes[0].Age);
                Assert.AreEqual(true, clientes[0].IsActive);

                Assert.AreEqual(2, clientes[1].Id);
                Assert.AreEqual("Walmart", clientes[1].Name);
                Assert.AreEqual(0, clientes[1].Age);
                Assert.AreEqual(false, clientes[1].IsActive);
            }
        }
    }
}
