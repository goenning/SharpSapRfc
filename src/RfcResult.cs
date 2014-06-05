using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpSapRfc
{
    public class RfcResult
    {
        private IRfcFunction function;
        internal RfcResult(IRfcFunction function)
        {
            this.function = function;
        }

        public T GetOutput<T>(string name)
        {
            object returnValue = function.GetValue(name);
            if (returnValue is IRfcStructure)
                return RfcStructureMapper.FromStructure<T>(returnValue as IRfcStructure);
            else
                returnValue = RfcStructureMapper.FromValue(typeof(T), returnValue);

            return (T)Convert.ChangeType(returnValue, typeof(T));
        }

        public IEnumerable<T> GetTable<T>(string name)
        {
            IRfcTable table = this.function.GetTable(name);
            List<T> returnTable = new List<T>(table.RowCount);
            for (int i = 0; i < table.RowCount; i++)
                returnTable.Add(RfcStructureMapper.FromStructure<T>(table[i]));

            return returnTable;
        }
    }
}
