using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using SharpSapRfc.Test.Model;
using Xunit;
using System.Linq;

namespace SharpSapRfc.Test.TestCases
{
    public class Soap_NestedTableTestCase : NestedTableTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new SoapSapRfcConnection("TST-SOAP");
        }
    }

    public class Plain_NestedTableTestCase : NestedTableTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new PlainSapRfcConnection("TST");
        }
    }

    public abstract class NestedTableTestCase
    {
        protected abstract SapRfcConnection GetConnection();

        [Fact]
        public void ExportStructureWithInnerTableTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_GET_ORDER",
                    new RfcParameter("i_id", 2)
                );

                var order = result.GetOutput<ZOrder>("e_order");
                Assert.Equal(2, order.Id);
                Assert.Equal(3, order.Items.Count());
            }
        }
    }
}
