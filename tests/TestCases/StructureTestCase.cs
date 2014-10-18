using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using SharpSapRfc.Test.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SharpSapRfc.Test.TestCases
{
    public class Soap_StructureTestCase : StructureTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new SoapSapRfcConnection("TST-SOAP");
        }
    }

    public class Plain_StructureTestCase : StructureTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            return new PlainSapRfcConnection("TST");
        }
    }

    public abstract class StructureTestCase
    {
        protected abstract SapRfcConnection GetConnection();

        [Fact]
        public void ImportStructureTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var customer = new ZCustomer { Id = 3, Name = "Microsoft", IsActive = true };
                var result = conn.ExecuteFunction("Z_SSRT_ADD_CUSTOMER", 
                    new RfcParameter("i_customer", customer)
                );

                string message = result.GetOutput<string>("e_success");
                Assert.Equal("Created:    3 - Microsoft - X", message);
            }
        }

        [Fact]
        public void ImportStructureTest_WithAnonymousType()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var customer = new ZCustomer { Id = 3, Name = "Microsoft", IsActive = true };
                var result = conn.ExecuteFunction("Z_SSRT_ADD_CUSTOMER", new
                {
                    i_customer = customer
                });

                string message = result.GetOutput<string>("e_success");
                Assert.Equal("Created:    3 - Microsoft - X", message);
            }
        }

        [Fact]
        public void ExportStructureTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_GET_CUSTOMER", 
                    new RfcParameter("i_id", 2)
                );

                var customer = result.GetOutput<ZCustomer>("e_customer");
                Assert.Equal(2, customer.Id);
                Assert.Equal("Walmart", customer.Name);
                Assert.Equal(false, customer.IsActive);
            }
        }

        [Fact]
        public void ExportTableTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_GET_ALL_CUSTOMERS");

                var customers = result.GetTable<ZCustomer>("t_customers");
                Assert.Equal(2, customers.Count());

                Assert.Equal(1, customers.ElementAt(0).Id);
                Assert.Equal("Apple Store", customers.ElementAt(0).Name);
                Assert.Equal(0, customers.ElementAt(0).Age);
                Assert.Equal(true, customers.ElementAt(0).IsActive);

                Assert.Equal(2, customers.ElementAt(1).Id);
                Assert.Equal("Walmart", customers.ElementAt(1).Name);
                Assert.Equal(0, customers.ElementAt(1).Age);
                Assert.Equal(false, customers.ElementAt(1).IsActive);
            }
        }

        [Fact]
        public void ExportTableCategoryTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                var result = conn.ExecuteFunction("Z_SSRT_GET_ALL_CUSTOMERS2");

                var customers = result.GetTable<ZCustomer>("e_customers");
                Assert.Equal(2, customers.Count());

                Assert.Equal(1, customers.ElementAt(0).Id);
                Assert.Equal("Apple Store", customers.ElementAt(0).Name);
                Assert.Equal(0, customers.ElementAt(0).Age);
                Assert.Equal(true, customers.ElementAt(0).IsActive);

                Assert.Equal(2, customers.ElementAt(1).Id);
                Assert.Equal("Walmart", customers.ElementAt(1).Name);
                Assert.Equal(0, customers.ElementAt(1).Age);
                Assert.Equal(false, customers.ElementAt(1).IsActive);
            }
        }

        [Fact]
        public void ChangingSingleStructureAsTableTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                ZCustomer customer = new ZCustomer() { Id = 1 };

                var result = conn.ExecuteFunction("Z_SSRT_QUERY_CUSTOMERS",
                    new RfcParameter("c_customers", customer)
                );

                var customers = result.GetTable<ZCustomer>("c_customers");
                Assert.Equal(1, customers.Count());

                Assert.Equal(1, customers.ElementAt(0).Id);
                Assert.Equal("Apple Store", customers.ElementAt(0).Name);
                Assert.Equal(0, customers.ElementAt(0).Age);
                Assert.Equal(true, customers.ElementAt(0).IsActive);
            }
        }

        [Fact]
        public void ChangingTableCategoryTest()
        {
            using (SapRfcConnection conn = this.GetConnection())
            {
                IEnumerable<ZCustomer> customers = new ZCustomer[] { 
                    new ZCustomer() { Id = 1 }
                }; 

                var result = conn.ExecuteFunction("Z_SSRT_QUERY_CUSTOMERS", 
                    new RfcParameter("c_customers", customers)
                );

                customers = result.GetTable<ZCustomer>("c_customers");
                Assert.Equal(1, customers.Count());

                Assert.Equal(1, customers.ElementAt(0).Id);
                Assert.Equal("Apple Store", customers.ElementAt(0).Name);
                Assert.Equal(0, customers.ElementAt(0).Age);
                Assert.Equal(true, customers.ElementAt(0).IsActive);
            }
        }
    }
}