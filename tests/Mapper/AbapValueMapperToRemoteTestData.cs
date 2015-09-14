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
    public class Soap_AbapValueMapperToRemoteTestData : AbapValueMapperToRemoteTestData
    {
        protected override List<object[]> AdditionalTestData()
        {
            return new List<object[]> { 
                 new object[] { "56.2", AbapDataType.DECIMAL, 56.2m },
            };
        }
    }

    public class Plain_AbapValueMapperToRemoteTestData : AbapValueMapperToRemoteTestData
    {
        protected override List<object[]> AdditionalTestData()
        {
            return new List<object[]> { 
                 new object[] { "56.2", AbapDataType.DECIMAL, 56.2m },
            };
        }
    }

    public class AbapValueMapperToRemoteTestData : IEnumerable<object[]>
    {

        protected virtual List<object[]> AdditionalTestData()
        {
            return new List<object[]>();
        }

        private List<object[]> testData;
        public AbapValueMapperToRemoteTestData()
        {
            this.testData = AdditionalTestData();
            this.testData.AddRange(new[] {
                new object[] { "X", AbapDataType.CHAR, true },
                new object[] { " ", AbapDataType.CHAR, false },
                    
                new object[] { "My test", AbapDataType.CHAR, "My test" },
                    
                new object[] { "12424", AbapDataType.DECIMAL, 12424m },
                new object[] { "464", AbapDataType.DECIMAL, 464m },

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
