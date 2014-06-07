using SAP.Middleware.Connector;
using SharpSapRfc.Test.Model;
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
                RfcValueMapper.FromRemoteValue(expectedType, remoteValue);
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
                    
                    new object[] { "My test", RfcDataType.CHAR, "My test" },
                    
                    new object[] { 12424m, RfcDataType.FLOAT, 12424m },
                    new object[] { 464m, RfcDataType.DECF16, 464m },
                    new object[] { 56m, RfcDataType.DECF34, 56m },

                    new object[] { "20140406", RfcDataType.DATE, new DateTime(2014, 4, 6) },
                    new object[] { "124253", RfcDataType.TIME, new DateTime(2014, 4, 6, 12, 42, 53) },

                    new object[] { "AVAL", RfcDataType.CHAR, MaterialState.Available },
                    new object[] { "BLOK", RfcDataType.CHAR, MaterialState.Blocked },
                    new object[] { "OOS", RfcDataType.CHAR, MaterialState.OutOfStock },
                    new object[] { "", RfcDataType.CHAR, default(MaterialState) },

                    new object[] { "1", RfcDataType.NUM, MaterialState.Available },
                    new object[] { "2", RfcDataType.NUM, MaterialState.Blocked },
                    new object[] { "3", RfcDataType.NUM, MaterialState.OutOfStock },
                    new object[] { "0", RfcDataType.NUM, default(MaterialState) },
                    
                    new object[] { 1, RfcDataType.INT1, MaterialState.Available },
                    new object[] { 2, RfcDataType.INT1, MaterialState.Blocked },
                    new object[] { 3, RfcDataType.INT1, MaterialState.OutOfStock },
                    new object[] { 0, RfcDataType.INT1, default(MaterialState) },
                    
                    new object[] { 1, RfcDataType.INT2, MaterialState.Available },
                    new object[] { 2, RfcDataType.INT2, MaterialState.Blocked },
                    new object[] { 3, RfcDataType.INT2, MaterialState.OutOfStock },
                    new object[] { 0, RfcDataType.INT2, default(MaterialState) },
                    
                    new object[] { 1, RfcDataType.INT4, MaterialState.Available },
                    new object[] { 2, RfcDataType.INT4, MaterialState.Blocked },
                    new object[] { 3, RfcDataType.INT4, MaterialState.OutOfStock },
                    new object[] { 0, RfcDataType.INT4, default(MaterialState) },
                    
                    new object[] { 1, RfcDataType.INT8, MaterialState.Available },
                    new object[] { 2, RfcDataType.INT8, MaterialState.Blocked },
                    new object[] { 3, RfcDataType.INT8, MaterialState.OutOfStock },
                    new object[] { 0, RfcDataType.INT8, default(MaterialState) }
                };
            }
        }
    }
}
