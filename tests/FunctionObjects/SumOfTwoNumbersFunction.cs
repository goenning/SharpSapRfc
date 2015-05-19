
namespace SharpSapRfc.Test.FunctionObjects
{
    public class SumOfTwoNumbersFunction : RfcFunctionObject<int>
    {
        public int First { get; private set; }
        public int Second { get; private set; }
        public int Total { get; private set; }

        public SumOfTwoNumbersFunction(int first, int second)
        {
            this.First = first;
            this.Second = second;
        }

        public override string FunctionName
        {
            get { return "Z_SSRT_SUM"; }
        }

        public override int GetOutput(RfcResult result)
        {
            return result.GetOutput<int>("e_result");
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
