using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace SharpSapRfc.Test
{
    public class RfcValueMapperTestCase
    {
        [Theory]
        [PropertyData("FromRemoteTestData")]
        public void FromRemoteTest(object expected, Type expectedType, object remoteValue)
        {
            object result = RfcValueMapper.FromRemoteValue(expectedType, remoteValue);
            Assert.IsType(expectedType, result);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> FromRemoteTestData
        {
            get
            {
                return new[]
                {
                    new object[] { true, typeof(Boolean), "X" },
                    new object[] { false, typeof(Boolean), " " },
                    new object[] { "My Name is", typeof(String), "My Name is" },
                    new object[] { 1, typeof(int), "0001" },
                    new object[] { 10, typeof(int), "10" },
                    new object[] { 15, typeof(int), 15 },
                    new object[] { new DateTime(1, 1, 1, 12, 42, 12), typeof(DateTime), "12:42:12" },
                    new object[] { new DateTime(1, 1, 1, 17, 9, 34), typeof(DateTime), "170934" },
                    new object[] { new DateTime(2014, 12, 10), typeof(DateTime), "2014-12-10" },
                    new object[] { new DateTime(2014, 7, 4), typeof(DateTime), "20140704" },
                    new object[] { -200m, typeof(Decimal), "-200" },
                    new object[] { 200.78m, typeof(Decimal), "200.78" },
                    new object[] { 200.78m, typeof(Decimal), "200,78" },
                    new object[] { 514646.89m, typeof(Decimal), "514646.89" },
                    new object[] { 514646.89m, typeof(Decimal), "514646,89" },
                    new object[] { 1988.89d, typeof(Double), "1.988,89" },
                    new object[] { -451d, typeof(Double), "-451" }
                };
            }
        }

        [Theory]
        [PropertyData("ToRemoteTestData")]
        public void ToRemoteValueTest(object expected, RfcDataType expectedType, object inputValue)
        {
            object result = RfcValueMapper.ToRemoteValue(expectedType, inputValue);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> ToRemoteTestData
        {
            get
            {
                return new[]
                {
                    new object[] { "X", RfcDataType.CHAR, true },
                    new object[] { " ", RfcDataType.CHAR, false },
                    new object[] { "20140406", RfcDataType.DATE, new DateTime(2014, 4, 6) },
                    new object[] { "124253", RfcDataType.TIME, new DateTime(2014, 4, 6, 12, 42, 53) }
                };
            }
        }
    }
}
