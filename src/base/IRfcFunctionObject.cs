
namespace SharpSapRfc
{
    public abstract class RfcFunctionObject<T>
    {
        public abstract string FunctionName { get; }
        public abstract T GetOutput(RfcResult result);

        public virtual object Parameters 
        { 
            get { return null; }
        }
    }
}
