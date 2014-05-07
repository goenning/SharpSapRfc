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
                var result = conn.ExecuteFunction("ZSOMA", new RfcImportParameter[] {
                    new RfcImportParameter("i_nro1", 2),
                    new RfcImportParameter("i_nro2", 4)
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
                var result = conn.ExecuteFunction("ZDIVIDE", new RfcImportParameter[] {
                    new RfcImportParameter("i_nro1", 5),
                    new RfcImportParameter("i_nro2", 2)
                });

                var divisao = result.GetExportParameter<decimal>("e_divisao");
                var resto = result.GetExportParameter<int>("e_resto");
                Assert.AreEqual(2.5m, divisao);
                Assert.AreEqual(1, resto);
            }
        }

        [TestMethod]
        public void ExceptionTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                try
                {
                    var result = conn.ExecuteFunction("ZDIVIDE", new RfcImportParameter[] {
                        new RfcImportParameter("i_nro1", 5),
                        new RfcImportParameter("i_nro2", 0)
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
                var result = conn.ExecuteFunction("Z_GET_CLIENTE", new RfcImportParameter[] {
                    new RfcImportParameter("i_codcli", 2)
                });

                var cliente = result.GetExportParameter<ZCliente>("e_cliente");
                Assert.AreEqual(2, cliente.Codigo);
                Assert.AreEqual("Padaria da Esquina", cliente.Nome);
            }
        }

        [TestMethod]
        public void TableExportTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                var result = conn.ExecuteFunction("Z_GET_CLIENTES");

                var clientes = result.GetExportTable<ZCliente>("t_clientes");
                Assert.AreEqual(2, clientes.Length);
                    
                Assert.AreEqual(1, clientes[0].Codigo);
                Assert.AreEqual("Mercado da Maria", clientes[0].Nome);

                Assert.AreEqual(2, clientes[1].Codigo);
                Assert.AreEqual("Padaria da Esquina", clientes[1].Nome);
            }
        }
    }
}
