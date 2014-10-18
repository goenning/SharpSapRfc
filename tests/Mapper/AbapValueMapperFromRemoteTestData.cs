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
    public class Soap_AbapValueMapperFromRemoteTestData : AbapValueMapperFromRemoteTestData
    {
        protected override List<object[]> AdditionalTestData()
        {
            return new List<object[]> { 
                new object[] { 200.78m, typeof(Decimal), "200.78" },
                new object[] { 514646.89m, typeof(Decimal), "514646.89" },
                new object[] { 1988.89d, typeof(Double), "1988.89" },
            };
        }
    }

    public class Plain_AbapValueMapperFromRemoteTestData : AbapValueMapperFromRemoteTestData
    {
        protected override List<object[]> AdditionalTestData()
        {
            return new List<object[]> { 
                new object[] { 200.78m, typeof(Decimal), "200,78" },
                new object[] { 514646.89m, typeof(Decimal), "514646,89" },
                new object[] { 1988.89d, typeof(Double), "1988,89" },
            };
        }
    }

    public class AbapValueMapperFromRemoteTestData : IEnumerable<object[]>
    {

        protected virtual List<object[]> AdditionalTestData()
        {
            return new List<object[]>();
        }

        private List<object[]> testData;
        public AbapValueMapperFromRemoteTestData()
        {
            this.testData = AdditionalTestData();
            this.testData.AddRange(new[] {
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
                
                new object[] { 0m, typeof(Decimal), null },
                new object[] { null, typeof(Decimal?), null },
                new object[] { 0m, typeof(Decimal), "" },
                new object[] { null, typeof(Decimal?), "" },
                new object[] { -200m, typeof(Decimal), "-200" },
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
