using SAP.Middleware.Connector;
using System.Collections.Generic;

namespace SharpSapRfc.Plain
{
    public class PlainRfcResult : RfcResult
    {
        private IRfcFunction function;
        internal PlainRfcResult(IRfcFunction function)
        {
            this.function = function;
        }

        public override T GetOutput<T>(string name)
        {
            object returnValue = function.GetValue(name);
            if (returnValue is IRfcStructure)
                return RfcStructureMapper.FromStructure<T>(returnValue as IRfcStructure);

            return (T)AbapValueMapper.FromRemoteValue(typeof(T), returnValue);
        }

        public override IEnumerable<T> GetTable<T>(string name)
        {
            IRfcTable table = this.function.GetTable(name);
            List<T> returnTable = new List<T>(table.RowCount);
            for (int i = 0; i < table.RowCount; i++)
                returnTable.Add(RfcStructureMapper.FromStructure<T>(table[i]));

            return returnTable;
        }
    }
}
