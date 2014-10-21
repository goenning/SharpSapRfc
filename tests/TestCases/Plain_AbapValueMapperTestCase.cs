using SharpSapRfc.Plain;
using SharpSapRfc.Test.Mapper;
using SharpSapRfc.Types;
using System;
using Xunit;
using Xunit.Extensions;

namespace SharpSapRfc.Test.TestCases
{
    public class Plain_AbapValueMapperTestCase
    {
        private PlainRfcValueMapper valueMapper;
        public Plain_AbapValueMapperTestCase()
        {
            this.valueMapper = new PlainRfcValueMapper();
        }

        [Theory]
        [ClassData(typeof(Plain_AbapValueMapperFromRemoteTestData))]
        public void FromRemoteTest(object expected, Type expectedType, object remoteValue)
        {
            object result = this.valueMapper.FromRemoteValue(expectedType, remoteValue);
            if (result != null)
                Assert.IsType(expectedType, result);
            Assert.Equal(expected, result);
        }

        [Theory]
        [ClassData(typeof(Plain_AbapValueMapperFromRemoteExceptionTestData))]
        public void FromRemoteExceptionTest(Type expectedType, object remoteValue)
        {
            Assert.Throws<RfcMappingException>(() =>
            {
                this.valueMapper.FromRemoteValue(expectedType, remoteValue);
            });
        }

        [Theory]
        [ClassData(typeof(Plain_AbapValueMapperToRemoteTestData))]
        public void ToRemoteValueTest(object expected, AbapDataType expectedType, object inputValue)
        {
            object result = this.valueMapper.ToRemoteValue(expectedType, inputValue);
            Assert.Equal(expected, result);
        }
    }
}
