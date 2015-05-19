using SAP.Middleware.Connector;
using System;

namespace SharpSapRfc.Plain
{
    public class PlainRfcPreparedFunction : RfcPreparedFunction
    {
        private string functionName;
        private RfcRepository repository;
        private RfcDestination destination;
        private PlainRfcStructureMapper structureMapper;

        public PlainRfcPreparedFunction(string functionName,
                                        PlainRfcStructureMapper structureMapper, 
                                        RfcRepository repository, 
                                        RfcDestination destination)
        {
            this.functionName = functionName;
            this.repository = repository;
            this.destination = destination;
            this.structureMapper = structureMapper;
        }

        public override RfcResult Execute()
        {
            IRfcFunction function = this.repository.CreateFunction(functionName);
            
            try
            {
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
                            IRfcStructure structure = this.structureMapper.CreateStructure(structureMetadata, parameter.Value);
                            function.SetValue(parameter.Name, structure);
                            break;
                        case RfcDataType.TABLE:
                            RfcTableMetadata tableMetadata = function.GetTable(idx).Metadata;
                            IRfcTable table = this.structureMapper.CreateTable(tableMetadata, parameter.Value);
                            function.SetValue(parameter.Name, table);
                            break;
                        default:
                            object formattedValue = this.structureMapper.ToRemoteValue(function.Metadata[idx].GetAbapDataType(), parameter.Value);
                            function.SetValue(parameter.Name, formattedValue);
                            break;
                    }
                }
                function.Invoke(this.destination);
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException() is SharpRfcException)
                    throw ex;

                throw new SharpRfcCallException(ex.Message, function.ToString(), ex);
            }

            return new PlainRfcResult(function, this.structureMapper);
        }
    }
}
