using SAP.Middleware.Connector;
using System.Collections.Generic;

namespace SharpSapRfc.Plain
{
    public class PlainRfcResult : RfcResult
    {
        private IRfcFunction function;
        private PlainRfcStructureMapper structureMapper;

        internal PlainRfcResult(IRfcFunction function, PlainRfcStructureMapper structureMapper)
        {
            this.function = function;
            this.structureMapper = structureMapper;
        }

        public override T GetOutput<T>(string name)
        {
            object returnValue = function.GetValue(name);
            if (returnValue is IRfcStructure)
                return this.structureMapper.FromStructure<T>(returnValue as IRfcStructure);

            return (T)this.structureMapper.FromRemoteValue(typeof(T), returnValue);
        }

        public override IEnumerable<T> GetTable<T>(string name)
        {
            IRfcTable table = this.function.GetTable(name);
            List<T> returnTable = new List<T>(table.RowCount);
            for (int i = 0; i < table.RowCount; i++)
                returnTable.Add(this.structureMapper.FromStructure<T>(table[i]));

            return returnTable;
        }
    }
}
