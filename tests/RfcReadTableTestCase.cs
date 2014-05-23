using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSapRfc.Test.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Test
{
    [TestClass]
    public class RfcReadTableTestCase
    {
        [TestMethod]
        public void ReadAllEntriesTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR");
                Assert.AreEqual(18, scarr.Count());
            }
        }

        [TestMethod]
        public void ReadAllFieldsTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR");

                var aa = scarr.FirstOrDefault(x => x.Code == "AA");
                Assert.AreEqual("AA", aa.Code);
                Assert.AreEqual("American Airlines", aa.Name);
                Assert.AreEqual("USD", aa.Currency);
                Assert.AreEqual("http://www.aa.com", aa.Url);
            }
        }

        [TestMethod]
        public void ReadSingleFieldTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR", new string[] { "CARRID" });

                var aa = scarr.FirstOrDefault(x => x.Code == "AA");
                Assert.AreEqual("AA", aa.Code);
                Assert.AreEqual(null, aa.Name);
                Assert.AreEqual(null, aa.Currency);
                Assert.AreEqual(null, aa.Url);
            }
        }
    }
}
