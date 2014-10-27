using SAP.Middleware.Connector;
using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using SharpSapRfc.Test.Model;
using System;
using System.Drawing;
using System.IO;
using Xunit;

namespace SharpSapRfc.Test.TestCases
{
    public class Soap_SimpleTestCase : SimpleTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new SoapSapRfcConnection("TST-SOAP");
        }
    }

    public class Plain_SimpleTestCase : SimpleTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new PlainSapRfcConnection("TST");
        }
    }

    public abstract class SimpleTestCase
    {
        protected abstract SapRfcConnection GetConnection();

        [Fact]
        public void ExportSingleParameterTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_SUM",
                    new RfcParameter("i_num1", 2),
                    new RfcParameter("i_num2", 4)
                );

                var total = result.GetOutput<int>("e_result");
                Assert.Equal(6, total);
            }
        }

        [Fact]
        public void ExportSingleParameterTest_WithAnonymousType()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_SUM", new
                {
                    i_num1 = 2,
                    i_num2 = 7
                });

                var total = result.GetOutput<int>("e_result");
                Assert.Equal(9, total);
            }
        }

        [Fact]
        public void ChangingSingleParameterTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_ADD",
                    new RfcParameter("i_add", 4),
                    new RfcParameter("c_num", 4)
                );

                var total = result.GetOutput<int>("c_num");
                Assert.Equal(8, total);
            }
        }

        [Fact]
        public void ExportMultipleParametersTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_DIVIDE",
                    new RfcParameter("i_num1", 5),
                    new RfcParameter("i_num2", 2)
                );

                var quotient = result.GetOutput<decimal>("e_quotient");
                var remainder = result.GetOutput<int>("e_remainder");
                Assert.Equal(2.5m, quotient);
                Assert.Equal(1, remainder);
            }
        }

        [Fact]
        public void AllTypesInOutTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_IN_OUT", new
                {
                    I_NAME = "",
                    I_ID = 2,
                    I_INT1 = 200,
                    I_FLOAT = 20.12f,
                    I_DEC_3_2 = 4.52,
                    I_DEC_23_4 = 999999999.999,
                    I_DEC_30_7 = 999999999.9999,
                    I_DATUM = new DateTime(2014, 4, 6),
                    I_UZEIT = new DateTime(1, 1, 1, 12, 10, 53),
                    i_active = true,
                    i_multiple_id = new int[] { 10, 20, 30 },
                    i_multiple_name = new string[] { "A", "B", "C", "D" },
                    i_mara = new ZMaraSingleDateTime { Id = 4, DateTime = new DateTime(2014, 4, 6, 12, 10, 53) }
                });

                Assert.Equal("", result.GetOutput<string>("E_NAME"));
                Assert.Equal(2, result.GetOutput<int>("E_ID"));
                Assert.Equal(200, result.GetOutput<byte>("E_INT1"));
                Assert.Equal(4.52m, result.GetOutput<decimal>("E_DEC_3_2"));
                Assert.Equal(999999999.999m, result.GetOutput<decimal>("E_DEC_23_4"));
                Assert.Equal(999999999.9999m, result.GetOutput<decimal>("E_DEC_30_7"));
                Assert.Equal(20.12f, result.GetOutput<double>("E_FLOAT"), 1);
                Assert.Equal(new DateTime(2014, 4, 6), result.GetOutput<DateTime>("E_DATUM"));
                Assert.Equal(new DateTime(1, 1, 1, 12, 10, 53), result.GetOutput<DateTime>("E_UZEIT"));
                Assert.Equal(true, result.GetOutput<bool>("e_active"));

                Assert.Equal(4, result.GetOutput<int>("e_mara_id"));
                Assert.Equal(new int[] { 10, 20, 30 }, result.GetTable<int>("e_multiple_id"));
                Assert.Equal(new string[] { "A", "B", "C", "D" }, result.GetTable<string>("e_multiple_name"));
                Assert.Equal(new DateTime(2014, 4, 6), result.GetOutput<DateTime>("e_mara_datum"));
                Assert.Equal(new DateTime(1, 1, 1, 12, 10, 53), result.GetOutput<DateTime>("e_mara_UZEIT"));
            }
        }

        [Fact]
        public void CustomExceptionOnInvalidParameter()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                Exception ex = Assert.Throws<UnknownRfcParameterException>(() =>
                {
                    var result = conn.ExecuteFunction("Z_SSRT_SUM",
                        new RfcParameter("i_num1", 2),
                        new RfcParameter("i_num2", 4),
                        new RfcParameter("i_num3", 4)
                    );
                });
                Assert.Equal("Parameter I_NUM3 was not found on function Z_SSRT_SUM.", ex.Message);
            }
        }

        [Fact]
        public void NullDateInOutTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_IN_OUT", new
                {
                    I_DATUM = (DateTime?)null
                });

                Assert.Equal(null, result.GetOutput<DateTime?>("E_DATUM"));
            }
        }

        [Fact]
        public void NullDateInOutTest_NonNullableStruct()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_IN_OUT", new
                {
                    I_DATUM = (DateTime?)null
                });

                Assert.Equal(DateTime.MinValue, result.GetOutput<DateTime>("E_DATUM"));
            }
        }

        [Fact]
        public void MinDateInOutTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_IN_OUT", new
                {
                    I_DATUM = DateTime.MinValue
                });

                Assert.Equal(DateTime.MinValue, result.GetOutput<DateTime>("E_DATUM"));
            }
        }

        [Fact]
        public void MaxDateInOutTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_IN_OUT", new
                {
                    I_DATUM = DateTime.MaxValue
                });

                Assert.Equal(DateTime.MaxValue.Date, result.GetOutput<DateTime>("E_DATUM"));
            }
        }

        [Fact]
        public void EmptyDateOutTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_EMPTY_DATE");
                Assert.Equal(DateTime.MinValue, result.GetOutput<DateTime>("E_DATUM"));
            }
        }

        [Fact]
        public void NullDateOutTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_EMPTY_DATE");
                Assert.Equal(null, result.GetOutput<DateTime?>("E_DATUM"));
            }
        }

        [Fact]
        public void ExceptionTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                Exception ex = Assert.Throws(typeof(SharpRfcException), () =>
                {
                    var result = conn.ExecuteFunction("Z_SSRT_DIVIDE",
                        new RfcParameter("i_num1", 5),
                        new RfcParameter("i_num2", 0)
                    );
                });
                Assert.Equal("DIVIDE_BY_ZERO", ex.Message);
            }
        }

        [Fact]
        public void ExportBinaryParameterAsByteArrayTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_GET_BMP_IMAGE", new
                {
                    i_object = "GRAPHICS",
                    i_name = "IDES_LOGO",
                    i_id = "BMAP",
                    i_btype = "BMON"
                });

                var bytes = result.GetOutput<byte[]>("e_image");
                using (Bitmap local = (Bitmap)Bitmap.FromFile(@"Assets\IDES_LOGO.bmp"))
                {
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        using (Bitmap remote = (Bitmap)Bitmap.FromStream(ms))
                        {
                            ImageAssert.AreEqual(local, remote);
                        }
                    }
                }
            }
        }

        [Fact]
        public void ExportBinaryParameterAsStreamTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_GET_BMP_IMAGE", new
                {
                    i_object = "GRAPHICS",
                    i_name = "IDES_LOGO",
                    i_id = "BMAP",
                    i_btype = "BMON"
                });

                using (var stream = result.GetOutput<Stream>("e_image"))
                {
                    using (Bitmap remote = (Bitmap)Bitmap.FromStream(stream))
                    {
                        using (Bitmap local = (Bitmap)Bitmap.FromFile(@"Assets\IDES_LOGO.bmp"))
                        {
                            ImageAssert.AreEqual(local, remote);
                        }
                    }
                }
            }
        }
    }
}
