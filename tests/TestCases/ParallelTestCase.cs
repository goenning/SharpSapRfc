using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using System.Threading.Tasks;
using Xunit;

namespace SharpSapRfc.Test.TestCases
{
    public class Soap_ParallelTestCase : ParallelTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new SoapSapRfcConnection("TST-SOAP");
        }
    }

    public class Plain_ParallelTestCase : ParallelTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new PlainSapRfcConnection("TST");
        }
    }

    public abstract class ParallelTestCase
    {
        protected abstract SapRfcConnection GetConnection();

        [Fact]
        public void TenTimesTest()
        {
            Parallel.For(1, 10, (int idx) =>
            {
                using (SapRfcConnection conn = this.GetConnection())
                {
                    var result = conn.ExecuteFunction("Z_SSRT_SUM",
                        new RfcParameter("i_num1", 2),
                        new RfcParameter("i_num2", 4)
                    );
                }
            });
            Task.WaitAll();
        }
    }
}
