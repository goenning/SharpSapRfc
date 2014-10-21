using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using SharpSapRfc.Test.Model;
using SharpSapRfc.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Extensions;

namespace SharpSapRfc.Test.Mapper
{
    public class Soap_AbapValueMapperFromRemoteExceptionTestData : AbapValueMapperFromRemoteExceptionTestData
    {
    }

    public class Plain_AbapValueMapperFromRemoteExceptionTestData : AbapValueMapperFromRemoteExceptionTestData
    {
    }

    public class AbapValueMapperFromRemoteExceptionTestData : IEnumerable<object[]>
    {

        protected virtual List<object[]> AdditionalTestData()
        {
            return new List<object[]>();
        }

        private List<object[]> testData;
        public AbapValueMapperFromRemoteExceptionTestData()
        {
            this.testData = AdditionalTestData();
            this.testData.AddRange(new[] {
                new object[] { typeof(MaterialState), 0 },
                new object[] { typeof(MaterialState), "0000" },
                new object[] { typeof(MaterialState), "" },
                new object[] { typeof(MaterialState), "BLAH" }
            });
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return this.testData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.testData.GetEnumerator();
        }
    }
}
