using SharpSapRfc.Test.Model;
using System.Collections.Generic;

namespace SharpSapRfc.Test.FunctionObjects
{
    public class GetAllCustomersFunction : RfcFunctionObject<IEnumerable<ZCustomer>>
    {
        public override string FunctionName
        {
            get { return "Z_SSRT_GET_ALL_CUSTOMERS2"; }
        }

        public override IEnumerable<ZCustomer> GetOutput(RfcResult result)
        {
            return result.GetTable<ZCustomer>("e_customers");
        }
    }
}
