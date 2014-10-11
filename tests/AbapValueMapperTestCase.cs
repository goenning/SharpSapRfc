using SAP.Middleware.Connector;
using SharpSapRfc.Plain;
using SharpSapRfc.Test.Model;
using SharpSapRfc.Types;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace SharpSapRfc.Test
{
    public class AbapValueMapperTestCase
    {
        [Theory]
        [PropertyData("FromRemoteTestData")]
        public void FromRemoteTest(object expected, Type expectedType, object remoteValue)
        {
            object result = AbapValueMapper.FromRemoteValue(expectedType, remoteValue);
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
                    new object[] { false, typeof(Boolean), "" },

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
                    new object[] { -451d, typeof(Double), "-451" },

                    new object[] { MaterialState.Available, typeof(MaterialState), 1 },
                    new object[] { MaterialState.Blocked, typeof(MaterialState), 2 },
                    new object[] { MaterialState.OutOfStock, typeof(MaterialState), 3 },

                    new object[] { MaterialState.Available, typeof(MaterialState), "0001" },
                    new object[] { MaterialState.Blocked, typeof(MaterialState), "0002" },
                    new object[] { MaterialState.OutOfStock, typeof(MaterialState), "0003" },

                    new object[] { MaterialState.Available, typeof(MaterialState), "AVAL" },
                    new object[] { MaterialState.Blocked, typeof(MaterialState), "BLOK" },
                    new object[] { MaterialState.OutOfStock, typeof(MaterialState), "OOS" }
                };
            }
        }

        [Theory]
        [PropertyData("FromRemoteExceptionTestData")]
        public void FromRemoteExceptionTest(Type expectedType, object remoteValue)
        {
            Assert.Throws<RfcMappingException>(() =>
            {
                AbapValueMapper.FromRemoteValue(expectedType, remoteValue);
            });
        }

        public static IEnumerable<object[]> FromRemoteExceptionTestData
        {
            get
            {
                return new[]
                {
                    new object[] { typeof(MaterialState), 0 },
                    new object[] { typeof(MaterialState), "0000" },
                    new object[] { typeof(MaterialState), "" },
                    new object[] { typeof(MaterialState), "BLAH" }
                };
            }
        }

        [Theory]
        [PropertyData("ToRemoteTestData")]
        public void ToRemoteValueTest(object expected, AbapDataType expectedType, object inputValue)
        {
            object result = AbapValueMapper.ToRemoteValue(expectedType, inputValue);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> ToRemoteTestData
        {
            get
            {
                return new[]
                {
                    new object[] { "X", AbapDataType.CHAR, true },
                    new object[] { " ", AbapDataType.CHAR, false },
                    
                    new object[] { "My test", AbapDataType.CHAR, "My test" },
                    
                    new object[] { 12424m, AbapDataType.DECIMAL, 12424m },
                    new object[] { 464m, AbapDataType.DECIMAL, 464m },
                    new object[] { 56m, AbapDataType.DECIMAL, 56m },

                    new object[] { "20140406", AbapDataType.DATE, new DateTime(2014, 4, 6) },
                    new object[] { "124253", AbapDataType.TIME, new DateTime(2014, 4, 6, 12, 42, 53) },

                    new object[] { "AVAL", AbapDataType.CHAR, MaterialState.Available },
                    new object[] { "BLOK", AbapDataType.CHAR, MaterialState.Blocked },
                    new object[] { "OOS", AbapDataType.CHAR, MaterialState.OutOfStock },
                    new object[] { "", AbapDataType.CHAR, default(MaterialState) },

                    new object[] { "1", AbapDataType.NUMERIC, MaterialState.Available },
                    new object[] { "2", AbapDataType.NUMERIC, MaterialState.Blocked },
                    new object[] { "3", AbapDataType.NUMERIC, MaterialState.OutOfStock },
                    new object[] { "0", AbapDataType.NUMERIC, default(MaterialState) },
                    
                    new object[] { 1, AbapDataType.INTEGER, MaterialState.Available },
                    new object[] { 2, AbapDataType.INTEGER, MaterialState.Blocked },
                    new object[] { 3, AbapDataType.INTEGER, MaterialState.OutOfStock },
                    new object[] { 0, AbapDataType.INTEGER, default(MaterialState) }
                    
                };
            }
        }
    }
}
