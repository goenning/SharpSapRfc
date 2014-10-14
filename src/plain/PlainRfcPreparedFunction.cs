using SAP.Middleware.Connector;

namespace SharpSapRfc.Plain
{
    public class PlainRfcPreparedFunction : RfcPreparedFunction
    {
        private string functionName;
        private RfcRepository repository;
        private RfcDestination destination;

        public PlainRfcPreparedFunction(string functionName, RfcRepository repository, RfcDestination destination)
        {
            this.functionName = functionName;
            this.repository = repository;
            this.destination = destination;
        }

        public override RfcResult Execute()
        {
            IRfcFunction function = this.repository.CreateFunction(functionName);
            foreach (var parameter in this.parameters)
            {
                int idx = function.Metadata.TryNameToIndex(parameter.Name);
                if (idx == -1)
                    throw new UnknownRfcParameterException(parameter.Name, functionName);

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
                        object formattedValue = AbapValueMapper.ToRemoteValue(function.Metadata[idx].GetAbapDataType(), parameter.Value);
                        function.SetValue(parameter.Name, formattedValue);
                        break;
                }
            }

            try
            {
                function.Invoke(this.destination);
            }
            catch (RfcAbapException ex)
            {
                throw new RfcException(ex.Message, ex);
            }
            return new PlainRfcResult(function);
        }
    }
}
