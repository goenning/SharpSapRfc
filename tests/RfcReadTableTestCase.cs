using SharpSapRfc.Structure;
using SharpSapRfc.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpSapRfc.Test
{
    public class RfcReadTableTestCase
    {
        [Fact]
        public void ReadAllEntriesTest()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR");
                Assert.Equal(18, scarr.Count());
            }
        }

        [Fact]
        public void ReadAllFieldsTest()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR");

                var aa = scarr.FirstOrDefault(x => x.Code == "AA");
                Assert.Equal("AA", aa.Code);
                Assert.Equal("American Airlines", aa.Name);
                Assert.Equal("USD", aa.Currency);
                Assert.Equal("http://www.aa.com", aa.Url);
            }
        }

        [Fact]
        public void ReadSingleFieldTest()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR", fields: new string[] { "CARRID" });
                Assert.Equal(18, scarr.Count());

                var aa = scarr.FirstOrDefault(x => x.Code == "AA");
                Assert.Equal("AA", aa.Code);
                Assert.Equal(null, aa.Name);
                Assert.Equal(null, aa.Currency);
                Assert.Equal(null, aa.Url);
            }
        }

        [Fact]
        public void ReadSingleEntryTest()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR", count:1);
                Assert.Equal(1, scarr.Count());
                Assert.Equal("AA", scarr.ElementAt(0).Code);
                Assert.Equal("American Airlines", scarr.ElementAt(0).Name);
                Assert.Equal("USD", scarr.ElementAt(0).Currency);
                Assert.Equal("http://www.aa.com", scarr.ElementAt(0).Url);
            }
        }

        [Fact]
        public void ReadDeltaAirlineCompanyTest()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR", where: new string[] { "CARRID = 'DL'" });

                Assert.Equal(1, scarr.Count());
                Assert.Equal("DL", scarr.ElementAt(0).Code);
                Assert.Equal("Delta Airlines", scarr.ElementAt(0).Name);
            }
        }

        [Fact]
        public void ReadTwoFieldDeltaAirlineCompanyTest()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR", 
                    fields: new string[] { "CARRID", "CURRCODE" }, 
                    where: new string[] { "CARRID = 'DL'" }
                );

                Assert.Equal(1, scarr.Count());
                Assert.Equal("DL", scarr.ElementAt(0).Code);
                Assert.Equal("USD", scarr.ElementAt(0).Currency);

                Assert.Equal(null, scarr.ElementAt(0).Name);
                Assert.Equal(null, scarr.ElementAt(0).Url);
                Assert.Equal(0, scarr.ElementAt(0).Client);
            }
        }

        [Fact]
        public void WhenChar1IsFirstField()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                var objects = conn.ReadTable<RepositoryObject>("TADIR",
                    fields: new string[] { "DELFLAG", "OBJ_NAME" },
                    where: new string[] { "OBJECT = 'TABL'", "AND OBJ_NAME = 'TADIR'" } 
                );

                Assert.Equal(1, objects.Count());
                Assert.Equal("TADIR", objects.ElementAt(0).Name);
                Assert.Equal(false, objects.ElementAt(0).DeletionFlag);
            }
        }

        [Fact]
        public void WhenChar1IsLastField()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                var objects = conn.ReadTable<RepositoryObject>("TADIR",
                    fields: new string[] { "OBJ_NAME", "DELFLAG" },
                    where: new string[] { "OBJECT = 'TABL'", "AND OBJ_NAME = 'TADIR'" }
                );

                Assert.Equal(1, objects.Count());
                Assert.Equal("TADIR", objects.ElementAt(0).Name);
                Assert.Equal(false, objects.ElementAt(0).DeletionFlag);
            }
        }

        [Fact]
        public void ReadTableAllFieldsType()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                ZMara mara = null;
                var maras = conn.ReadTable<ZMara>("ZSSRT_MARA");

                Assert.Equal(3, maras.Count());

                mara = maras.ElementAt(0);
                Assert.Equal(1, mara.Id);
                Assert.Equal("AOC MONITOR", mara.Name);
                Assert.Equal(254.54m, mara.Price);
                Assert.Equal(DateTime.MinValue, mara.Date);
                Assert.Equal(DateTime.MinValue, mara.Time);
                Assert.Equal(true, mara.IsActive);
                Assert.Equal(MaterialState.Blocked, mara.State);

                mara = maras.ElementAt(1);
                Assert.Equal(2, mara.Id);
                Assert.Equal("KOBO GLO", mara.Name);
                Assert.Equal(64m, mara.Price);
                Assert.Equal(new DateTime(2014, 6, 4), mara.Date);
                Assert.Equal(new DateTime(0001, 1, 1, 15, 42, 22), mara.Time);
                Assert.Equal(true, mara.IsActive);
                Assert.Equal(MaterialState.OutOfStock, mara.State);

                mara = maras.ElementAt(2);
                Assert.Equal(3, mara.Id);
                Assert.Equal("AVELL NOTEBOOK", mara.Name);
                Assert.Equal(21253154.2464m, mara.Price);
                Assert.Equal(new DateTime(2000, 1, 4), mara.Date);
                Assert.Equal(new DateTime(0001, 1, 1, 10, 0, 23), mara.Time);
                Assert.Equal(false, mara.IsActive);
                Assert.Equal(MaterialState.Available, mara.State);
            }
        }

        [Fact]
        public void ReadDateTimeSingleField()
        {
            using (SapRfcConnection conn = new SapRfcConnection("TST"))
            {
                ZMaraSingleDateTime mara = null;
                var maras = conn.ReadTable<ZMaraSingleDateTime>("ZSSRT_MARA");

                Assert.Equal(3, maras.Count());

                mara = maras.ElementAt(0);
                Assert.Equal(1, mara.Id);
                Assert.Equal(DateTime.MinValue, mara.DateTime);

                mara = maras.ElementAt(1);
                Assert.Equal(2, mara.Id);
                Assert.Equal(new DateTime(2014, 6, 4, 15, 42, 22), mara.DateTime);

                mara = maras.ElementAt(2);
                Assert.Equal(3, mara.Id);
                Assert.Equal(new DateTime(2000, 1, 4, 10, 0, 23), mara.DateTime);
            }
        }
    }
}
