using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using SharpSapRfc.Test.FunctionObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpSapRfc.Test.TestCases
{
    public class Soap_FunctionObjectTestCase : FunctionObjectTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new SoapSapRfcConnection("TST-SOAP");
        }
    }

    public class Plain_FunctionObjectTestCase : FunctionObjectTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new PlainSapRfcConnection("TST");
        }
    }

    public abstract class FunctionObjectTestCase
    {
        protected abstract SapRfcConnection GetConnection();

        [Fact]
        public void ObjectFunctionTest()
        {
            using (SapRfcConnection conn = GetConnection())
            {
                var result = conn.ExecuteFunction(new DivideTwoNumbersFunction(9, 2));
                Assert.Equal(4.5m, result.Quotient);
                Assert.Equal(1, result.Remainder);
            }
        }

        [Fact]
        public void ObjectFunctionWithSingleReturnValueTest()
        {
            using (SapRfcConnection conn = GetConnection())
            {
                int value = conn.ExecuteFunction(new SumOfTwoNumbersFunction(2, 8));
                Assert.Equal(10, value);
            }
        }

        [Fact]
        public void ObjectFunctionWithTableReturnValueTest()
        {
            using (SapRfcConnection conn = GetConnection())
            {
                var customers = conn.ExecuteFunction(new GetAllCustomersFunction());
                Assert.Equal(2, customers.Count());
            }
        }
    }
}
