using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using SharpSapRfc.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpSapRfc.Test.TestCases
{
    public class Soap_FluentRfcReadTableTestCase : FluentRfcReadTableTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new SoapSapRfcConnection("TST-SOAP");
        }
    }

    public class Plain_FluentRfcReadTableTestCase : FluentRfcReadTableTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new PlainSapRfcConnection("TST");
        }
    }

    public abstract class FluentRfcReadTableTestCase
    {
        protected abstract SapRfcConnection GetConnection();

        [Fact]
        public void ReadAllEntriesTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var scarr = conn.Table<AirlineCompany>("SCARR").Read();
                Assert.Equal(18, scarr.Count());
            }
        }

        [Fact]
        public void ReadTwoFieldsTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var scarr = conn.Table<AirlineCompany>("SCARR").Select("CARRID", "CARRNAME").Read();
                Assert.Equal(18, scarr.Count());

                var aa = scarr.FirstOrDefault(x => x.Code == "AA");
                Assert.Equal("AA", aa.Code);
                Assert.Equal("American Airlines", aa.Name);
                Assert.Equal(null, aa.Currency);
                Assert.Equal(null, aa.Url);
            }
        }

        [Fact]
        public void ReadTwoFieldFromDeltaAirlineCompanyTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var scarr = conn.Table<AirlineCompany>("SCARR")
                                .Select("CARRID", "CURRCODE")
                                .Where("CARRID = 'DL'")
                                .Read();

                Assert.Equal(1, scarr.Count());
                Assert.Equal("DL", scarr.ElementAt(0).Code);
                Assert.Equal("USD", scarr.ElementAt(0).Currency);

                Assert.Equal(null, scarr.ElementAt(0).Name);
                Assert.Equal(null, scarr.ElementAt(0).Url);
                Assert.Equal(0, scarr.ElementAt(0).Client);
            }
        }

        [Fact]
        public void StrongTypeEqualsConditionTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var scarr = conn.Table<AirlineCompany>("SCARR")
                                .Select("CARRID", "CURRCODE")
                                .Where("CARRID", RfcReadTableOption.Equals, "DL")
                                .Read();

                Assert.Equal(1, scarr.Count());
                Assert.Equal("DL", scarr.ElementAt(0).Code);
                Assert.Equal("USD", scarr.ElementAt(0).Currency);

                Assert.Equal(null, scarr.ElementAt(0).Name);
                Assert.Equal(null, scarr.ElementAt(0).Url);
                Assert.Equal(0, scarr.ElementAt(0).Client);
            }
        }

        [Fact]
        public void StrongTypeNotEqualsConditionTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var scarr = conn.Table<AirlineCompany>("SCARR")
                                .Select("CARRID", "CURRCODE")
                                .Where("CARRID", RfcReadTableOption.NotEquals, "DL")
                                .Read();

                foreach (var company in scarr)
                {
                    Assert.NotEqual("DL", company.Code);
                }
            }
        }

        [Fact]
        public void StrongTypeGreaterAndLessThanConditionTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var flights = conn.Table<Flight>("SPFLI")
                                  .Where("DISTANCE", RfcReadTableOption.GreaterThan, 8000)
                                  .And("DISTANCE", RfcReadTableOption.LessThan, 10000)
                                  .Read();

                Assert.Equal(4, flights.Count());
            }
        }

        [Fact]
        public void StrongTypeGreaterEqualAndLessEqualThanConditionTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var flights = conn.Table<Flight>("SPFLI")
                                  .Where("DISTANCE", RfcReadTableOption.GreaterOrEqualThan, 6030)
                                  .And("DISTANCE", RfcReadTableOption.LessOrEqualThan, 9100)
                                  .Read();

                Assert.Equal(12, flights.Count());
            }
        }

        [Fact]
        public void ReadWithTwoAndConditionsTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var scarr = conn.Table<AirlineCompany>("SCARR")
                                .Select("CARRID", "CURRCODE")
                                .Where("CARRID = 'DL'")
                                .And("CURRCODE = 'BRL'")
                                .Read();

                Assert.Equal(0, scarr.Count());
            }
        }

        [Fact]
        public void ReadWithTwoOrConditionsTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var scarr = conn.Table<AirlineCompany>("SCARR")
                                .Select("CARRID", "CURRCODE")
                                .Where("CARRID = 'DL'")
                                .Or("CARRID = 'AA'")
                                .Read();

                Assert.Equal(2, scarr.Count());
            }
        }

        [Fact]
        public void TakeFirstCompanyTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var scarr = conn.Table<AirlineCompany>("SCARR").Take(1).Read();
                Assert.Equal(1, scarr.Count());
            }
        }

        [Fact]
        public void FirstCompanyShouldBeDifferentThanSecondCompanyTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var firstCompany = conn.Table<AirlineCompany>("SCARR").Take(1).ReadOne();
                var secondCompany = conn.Table<AirlineCompany>("SCARR").Skip(1).Take(1).ReadOne();

                Assert.NotEqual(firstCompany.Code, secondCompany.Code);
            }
        }
    }
}
