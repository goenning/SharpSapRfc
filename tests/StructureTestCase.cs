using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSapRfc.Test.Structures;
using System.Collections.Generic;
using System.Linq;

namespace SharpSapRfc.Test
{
    [TestClass]
    public class StructureTestCase
    {
        [TestMethod]
        public void ImportStructureTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                var customer = new ZCustomer { Id = 3, Name = "Microsoft", IsActive = true };
                var result = conn.ExecuteFunction("Z_SSRT_ADD_CUSTOMER", 
                    new RfcParameter("i_customer", customer)
                );

                string message = result.GetOutput<string>("e_success");
                Assert.AreEqual("Created:    3 - Microsoft - X", message);
            }
        }

        [TestMethod]
        public void ExportStructureTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                var result = conn.ExecuteFunction("Z_SSRT_GET_CUSTOMER", 
                    new RfcParameter("i_id", 2)
                );

                var customer = result.GetOutput<ZCustomer>("e_customer");
                Assert.AreEqual(2, customer.Id);
                Assert.AreEqual("Walmart", customer.Name);
                Assert.AreEqual(false, customer.IsActive);
            }
        }

        [TestMethod]
        public void ExportTableTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                var result = conn.ExecuteFunction("Z_SSRT_GET_ALL_CUSTOMERS");

                var customers = result.GetTable<ZCustomer>("t_customers");
                Assert.AreEqual(2, customers.Count());

                Assert.AreEqual(1, customers.ElementAt(0).Id);
                Assert.AreEqual("Apple Store", customers.ElementAt(0).Name);
                Assert.AreEqual(0, customers.ElementAt(0).Age);
                Assert.AreEqual(true, customers.ElementAt(0).IsActive);

                Assert.AreEqual(2, customers.ElementAt(1).Id);
                Assert.AreEqual("Walmart", customers.ElementAt(1).Name);
                Assert.AreEqual(0, customers.ElementAt(1).Age);
                Assert.AreEqual(false, customers.ElementAt(1).IsActive);
            }
        }

        [TestMethod]
        public void ExportTableCategoryTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                var result = conn.ExecuteFunction("Z_SSRT_GET_ALL_CUSTOMERS2");

                var customers = result.GetTable<ZCustomer>("e_customers");
                Assert.AreEqual(2, customers.Count());

                Assert.AreEqual(1, customers.ElementAt(0).Id);
                Assert.AreEqual("Apple Store", customers.ElementAt(0).Name);
                Assert.AreEqual(0, customers.ElementAt(0).Age);
                Assert.AreEqual(true, customers.ElementAt(0).IsActive);

                Assert.AreEqual(2, customers.ElementAt(1).Id);
                Assert.AreEqual("Walmart", customers.ElementAt(1).Name);
                Assert.AreEqual(0, customers.ElementAt(1).Age);
                Assert.AreEqual(false, customers.ElementAt(1).IsActive);
            }
        }

        [TestMethod]
        public void ChangingTableCategoryTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                IEnumerable<ZCustomer> customers = new ZCustomer[] { 
                    new ZCustomer() { Id = 1 }
                }; 

                var result = conn.ExecuteFunction("Z_SSRT_QUERY_CUSTOMERS", 
                    new RfcParameter("c_customers", customers)
                );

                customers = result.GetTable<ZCustomer>("c_customers");
                Assert.AreEqual(1, customers.Count());

                Assert.AreEqual(1, customers.ElementAt(0).Id);
                Assert.AreEqual("Apple Store", customers.ElementAt(0).Name);
                Assert.AreEqual(0, customers.ElementAt(0).Age);
                Assert.AreEqual(true, customers.ElementAt(0).IsActive);
            }
        }

        [TestMethod]
        public void ChangingMultipleRowsTableCategoryTest()
        {
            using (SharpSapRfcConnection conn = new SharpSapRfcConnection(""))
            {
                IEnumerable<ZCustomer> customers = new ZCustomer[] { 
                    new ZCustomer() { Id = 1 },
                    new ZCustomer() { Id = 2 }
                };

                var result = conn.ExecuteFunction("Z_SSRT_QUERY_CUSTOMERS",
                    new RfcParameter("c_customers", customers)
                );

                customers = result.GetTable<ZCustomer>("c_customers");
                Assert.AreEqual(2, customers.Count());

                Assert.AreEqual(1, customers.ElementAt(0).Id);
                Assert.AreEqual("Apple Store", customers.ElementAt(0).Name);
                Assert.AreEqual(0, customers.ElementAt(0).Age);
                Assert.AreEqual(true, customers.ElementAt(0).IsActive);

                Assert.AreEqual(2, customers.ElementAt(1).Id);
                Assert.AreEqual("Walmart", customers.ElementAt(1).Name);
                Assert.AreEqual(0, customers.ElementAt(1).Age);
                Assert.AreEqual(false, customers.ElementAt(1).IsActive);
            }
        }
    }
}
