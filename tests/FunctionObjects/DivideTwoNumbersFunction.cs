
namespace SharpSapRfc.Test.FunctionObjects
{
    public class DivideTwoNumbersFunction : RfcFunctionObject<DivideTwoNumbersFunction>
    {
        public int First { get; private set; }
        public int Second { get; private set; }
        public decimal Quotient { get; private set; }
        public int Remainder { get; private set; }

        public DivideTwoNumbersFunction(int first, int second)
        {
            this.First = first;
            this.Second = second;
        }

        public override string FunctionName
        {
            get { return "Z_SSRT_DIVIDE"; }
        }

        public override DivideTwoNumbersFunction GetOutput(RfcResult result)
        {
            this.Quotient = result.GetOutput<decimal>("e_quotient");
            this.Remainder = result.GetOutput<int>("e_remainder");
            return this;
        }

        public override object Parameters
        {
            get 
            {
                return new
                {
                    i_num1 = this.First,
                    i_num2 = this.Second
                };
            }
        }
    }
}
