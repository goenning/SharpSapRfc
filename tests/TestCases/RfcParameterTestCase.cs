using SharpSapRfc.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpSapRfc.Test.TestCases
{
    public class RfcParameterTestCase
    {
        [Fact]
        public void SimpleTypeTest() 
        {
            RfcParameter customer = new RfcParameter("ID", 2);
            Assert.Equal("ID: 2", customer.ToString());
        }

        [Fact]
        public void ComplexTypeTest()
        {
            RfcParameter customer = new RfcParameter("CUSTOMER", new ZCustomer { Id = 5 });
            Assert.Equal("CUSTOMER: { ID: 5, NAME: null, ACTIVE: False, AGE: 0 }", customer.ToString());
        }

        [Fact]
        public void TwoFieldsComplexTypeTest()
        {
            RfcParameter customer = new RfcParameter("CUSTOMER", new ZCustomer { Id = 5, Name = "Walmart" });
            Assert.Equal("CUSTOMER: { ID: 5, NAME: Walmart, ACTIVE: False, AGE: 0 }", customer.ToString());
        }

        [Fact]
        public void ArrayTest()
        {
            RfcParameter customer = new RfcParameter("CUSTOMERS", new ZCustomer[] { 
                new ZCustomer { Id = 2 },
                new ZCustomer { Id = 5 }
            });
            Assert.Equal("CUSTOMERS: [ { ID: 2, NAME: null, ACTIVE: False, AGE: 0 }, { ID: 5, NAME: null, ACTIVE: False, AGE: 0 } ]", customer.ToString());
        }

        [Fact]
        public void EnumerableTest()
        {
            RfcParameter customer = new RfcParameter("CUSTOMERS", new List<ZCustomer>() { 
                new ZCustomer { Id = 2 },
                new ZCustomer { Id = 5 }
            });
            Assert.Equal("CUSTOMERS: [ { ID: 2, NAME: null, ACTIVE: False, AGE: 0 }, { ID: 5, NAME: null, ACTIVE: False, AGE: 0 } ]", customer.ToString());
        }
    }
}
