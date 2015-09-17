using SAP.Middleware.Connector;
using System;
using System.Text;

namespace SharpSapRfc.Plain
{
    public class PlainRfcPreparedFunction : RfcPreparedFunction
    {
        private RfcRepository repository;
        private RfcDestination destination;
        private PlainRfcStructureMapper structureMapper;

        public PlainRfcPreparedFunction(string functionName,
                                        PlainRfcStructureMapper structureMapper, 
                                        RfcRepository repository, 
                                        RfcDestination destination)
            : base(functionName)
        {
            this.repository = repository;
            this.destination = destination;
            this.structureMapper = structureMapper;
        }

        public override string ToString()
        {
            if (this.function != null)
            {
                StringBuilder importing = new StringBuilder();
                StringBuilder tables = new StringBuilder();
                StringBuilder changing = new StringBuilder();

                for (int i = 0; i < this.function.Metadata.ParameterCount; i++)
                {
                    var parameter = this.function.Metadata[i];
                    switch (parameter.Direction)
                    {
                        case RfcDirection.CHANGING:
                            changing.AppendFormat(" - {0} = {1}", parameter.Name, this.function.GetObject(i)).AppendLine();
                            break;
                        case RfcDirection.IMPORT:
                            importing.AppendFormat(" - {0} = {1}", parameter.Name, this.function.GetObject(i)).AppendLine();
                            break;
                        case RfcDirection.TABLES:
                            tables.AppendFormat("- {0} = {1}", parameter.Name, this.function.GetObject(i)).AppendLine();
                            break;
                    }
                }
                    
                StringBuilder output = new StringBuilder();
                output.AppendFormat("FUNCTION: {0}", this.FunctionName).AppendLine();
                output.Append(" IMPORTING: ").AppendLine().Append(importing);
                output.Append(" CHANGING: ").AppendLine().Append(changing);
                output.Append(" TABLES: ").AppendLine().Append(tables);
                return output.ToString();
            }

            return "<Empty Function>";
        }

        public override RfcPreparedFunction Prepare()
        {
            try 
            {
                this.function = this.repository.CreateFunction(this.FunctionName);
                foreach (var parameter in this.Parameters)
                {
                    int idx = this.function.Metadata.TryNameToIndex(parameter.Name);
                    if (idx == -1)
                        throw new UnknownRfcParameterException(parameter.Name, this.FunctionName);

                    RfcDataType pType = this.function.Metadata[idx].DataType;
                    switch (pType)
                    {
                        case RfcDataType.STRUCTURE:
                            RfcStructureMetadata structureMetadata = this.function.GetStructure(idx).Metadata;
                            IRfcStructure structure = this.structureMapper.CreateStructure(structureMetadata, parameter.Value);
                            this.function.SetValue(parameter.Name, structure);
                            break;
                        case RfcDataType.TABLE:
                            RfcTableMetadata tableMetadata = this.function.GetTable(idx).Metadata;
                            IRfcTable table = this.structureMapper.CreateTable(tableMetadata, parameter.Value);
                            this.function.SetValue(parameter.Name, table);
                            break;
                        default:
                            object formattedValue = this.structureMapper.ToRemoteValue(this.function.Metadata[idx].GetAbapDataType(), parameter.Value);
                            this.function.SetValue(parameter.Name, formattedValue);
                            break;
                    }
                }
                return this;
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException() is SharpRfcException)
                    throw ex;

                throw new SharpRfcCallException(ex.Message, function.ToString(), ex);
            }
        }

        public override RfcResult Execute()
        {            
            try
            {
                if (this.function == null)
                    this.Prepare();

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

        public IRfcFunction function { get; set; }
    }
}
