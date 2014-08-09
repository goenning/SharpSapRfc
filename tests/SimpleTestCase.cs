using SAP.Middleware.Connector;
using SharpSapRfc.Test.Model;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xunit;

namespace SharpSapRfc.Test
{
    public class SimpleTestCase
    {
        [Fact]
        public void ExportSingleParameterTest()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
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
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
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
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
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
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
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
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                var result = conn.ExecuteFunction("Z_SSRT_IN_OUT", new
                {
                    I_ID = 2,
                    I_PRICE = 464624.521,
                    I_DATUM = new DateTime(2014, 4, 6),
                    I_UZEIT = new DateTime(1, 1, 1, 12, 10, 53),
                    i_active = true,
                    i_mara = new ZMaraSingleDateTime { Id = 4, DateTime = new DateTime(2014, 4, 6, 12, 10, 53) }
                });

                Assert.Equal(2, result.GetOutput<int>("E_ID"));
                Assert.Equal(464624.521m, result.GetOutput<decimal>("E_PRICE"));
                Assert.Equal(new DateTime(2014, 4, 6), result.GetOutput<DateTime>("E_DATUM"));
                Assert.Equal(new DateTime(1, 1, 1, 12, 10, 53), result.GetOutput<DateTime>("E_UZEIT"));
                Assert.Equal(true, result.GetOutput<bool>("e_active"));

                Assert.Equal(4, result.GetOutput<int>("e_mara_id"));
                Assert.Equal(new DateTime(2014, 4, 6), result.GetOutput<DateTime>("e_mara_datum"));
                Assert.Equal(new DateTime(1, 1, 1, 12, 10, 53), result.GetOutput<DateTime>("e_mara_UZEIT"));
            }
        }

        [Fact]
        public void ExceptionTest()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                Assert.Throws(typeof(RfcAbapException), () =>
                {
                    var result = conn.ExecuteFunction("Z_SSRT_DIVIDE",
                        new RfcParameter("i_num1", 5),
                        new RfcParameter("i_num2", 0)
                    );
                });
            }
        }

        [Fact]
        public void ExportBinaryParameterAsByteArrayTest()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
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
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
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
