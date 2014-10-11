namespace SharpSapRfc
{
    public class UnknownRfcParameterException : RfcException
    {
        public UnknownRfcParameterException(string parameterName, string functionName)
            : base(string.Format("Parameter {0} was not found on function {1}.", parameterName, functionName))
        {
        }
    }
}
