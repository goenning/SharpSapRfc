using SharpSapRfc.Soap;
using SharpSapRfc.Test.Mapper;
using SharpSapRfc.Types;
using System;
using Xunit;
using Xunit.Extensions;

namespace SharpSapRfc.Test.TestCases
{
    public class Soap_AbapValueMapperTestCase
    {
        private SoapRfcValueMapper valueMapper;
        public Soap_AbapValueMapperTestCase()
        {
            this.valueMapper = new SoapRfcValueMapper();
        }

        [Theory]
        [ClassData(typeof(Soap_AbapValueMapperFromRemoteTestData))]
        public void FromRemoteTest(object expected, Type expectedType, object remoteValue)
        {
            object result = this.valueMapper.FromRemoteValue(expectedType, remoteValue);
            if (result != null)
                Assert.IsType(expectedType, result);
            Assert.Equal(expected, result);
        }

        [Theory]
        [ClassData(typeof(Soap_AbapValueMapperFromRemoteExceptionTestData))]
        public void FromRemoteExceptionTest(Type expectedType, object remoteValue)
        {
            Assert.Throws<RfcMappingException>(() =>
            {
                this.valueMapper.FromRemoteValue(expectedType, remoteValue);
            });
        }

        [Theory]
        [ClassData(typeof(Soap_AbapValueMapperToRemoteTestData))]
        public void ToRemoteValueTest(object expected, AbapDataType expectedType, object inputValue)
        {
            object result = this.valueMapper.ToRemoteValue(expectedType, inputValue);
            Assert.Equal(expected, result);
        }
    }
}
