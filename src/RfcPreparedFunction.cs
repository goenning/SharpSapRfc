using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SharpSapRfc
{
    public class RfcPreparedFunction
    {
        private string functionName;
        private SharpSapRfcConnection connection;
        private List<RfcParameter> rfcParameters;

        public RfcPreparedFunction(string functionName, SharpSapRfcConnection connection)
        {
            this.functionName = functionName;
            this.connection = connection;
            this.rfcParameters = new List<RfcParameter>();
        }

        public RfcPreparedFunction AddParameter(RfcParameter parameter)
        {
            this.rfcParameters.Add(parameter);
            return this;
        }

        public RfcPreparedFunction AddParameter(RfcParameter[] parameters)
        {
            this.rfcParameters.AddRange(parameters);
            return this;
        }

        public RfcPreparedFunction AddParameter(object parameters)
        {
            Type t = parameters.GetType();
            PropertyInfo[] properties = t.GetProperties();
            for (int i = 0; i < properties.Length; i++)
                this.rfcParameters.Add(new RfcParameter(properties[i].Name, properties[i].GetValue(parameters, null)));
            return this;
        }

        public RfcResult Execute()
        {
            IRfcFunction function = this.connection.Repository.CreateFunction(functionName);
            foreach (var parameter in this.rfcParameters)
            {
                int idx = function.Metadata.TryNameToIndex(parameter.Name);
                RfcDataType pType = function.Metadata[idx].DataType;
                switch (pType)
                {
                    case RfcDataType.STRUCTURE:
                        RfcStructureMetadata structureMetadata = function.GetStructure(idx).Metadata;
                        IRfcStructure structure = RfcStructureMapper.CreateStructure(structureMetadata, parameter.Value);
                        function.SetValue(parameter.Name, structure);
                        break;
                    case RfcDataType.TABLE:
                        RfcTableMetadata tableMetadata = function.GetTable(idx).Metadata;
                        IRfcTable table = RfcStructureMapper.CreateTable(tableMetadata, parameter.Value);
                        function.SetValue(parameter.Name, table);
                        break;
                    default:
                        function.SetValue(parameter.Name, parameter.Value);
                        break;
                }
            }

            function.Invoke(this.connection.Destination);
            return new RfcResult(function);
        }
    }
}
