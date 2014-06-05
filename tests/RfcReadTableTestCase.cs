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
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR");
                Assert.AreEqual(18, scarr.Count());
            }
        }

        [TestMethod]
        public void ReadAllFieldsTest()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
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
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
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
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
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
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
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
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
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

        [TestMethod]
        public void WhenChar1IsFirstField()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                var objects = conn.ReadTable<RepositoryObject>("TADIR",
                    fields: new string[] { "DELFLAG", "OBJ_NAME" },
                    where: new string[] { "OBJECT = 'TABL'", "AND OBJ_NAME = 'TADIR'" } 
                );

                Assert.AreEqual(1, objects.Count());
                Assert.AreEqual("TADIR", objects.ElementAt(0).Name);
                Assert.AreEqual(false, objects.ElementAt(0).DeletionFlag);
            }
        }

        [TestMethod]
        public void WhenChar1IsLastField()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                var objects = conn.ReadTable<RepositoryObject>("TADIR",
                    fields: new string[] { "OBJ_NAME", "DELFLAG" },
                    where: new string[] { "OBJECT = 'TABL'", "AND OBJ_NAME = 'TADIR'" }
                );

                Assert.AreEqual(1, objects.Count());
                Assert.AreEqual("TADIR", objects.ElementAt(0).Name);
                Assert.AreEqual(false, objects.ElementAt(0).DeletionFlag);
            }
        }

        [TestMethod]
        public void ReadTableAllFieldsType()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                ZMara mara = null;
                var maras = conn.ReadTable<ZMara>("ZSSRT_MARA");

                Assert.AreEqual(3, maras.Count());

                mara = maras.ElementAt(0);
                Assert.AreEqual(1, mara.Id);
                Assert.AreEqual("AOC MONITOR", mara.Name);
                Assert.AreEqual(254.54m, mara.Price);
                Assert.AreEqual(DateTime.MinValue, mara.Date);
                Assert.AreEqual(DateTime.MinValue, mara.Time);
                Assert.AreEqual(true, mara.IsActive);

                mara = maras.ElementAt(1);
                Assert.AreEqual(2, mara.Id);
                Assert.AreEqual("KOBO GLO", mara.Name);
                Assert.AreEqual(64m, mara.Price);
                Assert.AreEqual(new DateTime(2014, 6, 4), mara.Date);
                Assert.AreEqual(new DateTime(0001, 1, 1, 15, 42, 22), mara.Time);
                Assert.AreEqual(true, mara.IsActive);

                mara = maras.ElementAt(2);
                Assert.AreEqual(3, mara.Id);
                Assert.AreEqual("AVELL NOTEBOOK", mara.Name);
                Assert.AreEqual(21253154.2464m, mara.Price);
                Assert.AreEqual(new DateTime(2000, 1, 4), mara.Date);
                Assert.AreEqual(new DateTime(0001, 1, 1, 10, 0, 23), mara.Time);
                Assert.AreEqual(false, mara.IsActive);
            }
        }
    }
}
