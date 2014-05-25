using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSapRfc.Structure;
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
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR");
                Assert.AreEqual(18, scarr.Count());
            }
        }

        [TestMethod]
        public void ReadAllFieldsTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
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
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR", fields: new string[] { "CARRID" });
                Assert.AreEqual(18, scarr.Count());

                var aa = scarr.FirstOrDefault(x => x.Code == "AA");
                Assert.AreEqual("AA", aa.Code);
                Assert.AreEqual(null, aa.Name);
                Assert.AreEqual(null, aa.Currency);
                Assert.AreEqual(null, aa.Url);
            }
        }

        [TestMethod]
        public void ReadSingleEntryTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR", count:1);
                Assert.AreEqual(1, scarr.Count());
                Assert.AreEqual("AA", scarr.ElementAt(0).Code);
                Assert.AreEqual("American Airlines", scarr.ElementAt(0).Name);
                Assert.AreEqual("USD", scarr.ElementAt(0).Currency);
                Assert.AreEqual("http://www.aa.com", scarr.ElementAt(0).Url);
            }
        }

        [TestMethod]
        public void ReadDeltaAirlineCompanyTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR", where: new string[] { "CARRID = 'DL'" });

                Assert.AreEqual(1, scarr.Count());
                Assert.AreEqual("DL", scarr.ElementAt(0).Code);
                Assert.AreEqual("Delta Airlines", scarr.ElementAt(0).Name);
            }
        }

        [TestMethod]
        public void ReadTwoFieldDeltaAirlineCompanyTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection("TST"))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR", 
                    fields: new string[] { "CARRID", "CURRCODE" }, 
                    where: new string[] { "CARRID = 'DL'" }
                );

                Assert.AreEqual(1, scarr.Count());
                Assert.AreEqual("DL", scarr.ElementAt(0).Code);
                Assert.AreEqual("USD", scarr.ElementAt(0).Currency);

                Assert.AreEqual(null, scarr.ElementAt(0).Name);
                Assert.AreEqual(null, scarr.ElementAt(0).Url);
                Assert.AreEqual(0, scarr.ElementAt(0).Client);
            }
        }
    }
}
