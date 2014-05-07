using SAP.Middleware.Connector;
using System;

namespace SharpSapRfc
{
    public class SharpSapRfcConnection : IDisposable
    {
        private string connectionString;
        private RfcRepository repository;
        private RfcDestination destination;

        public SharpSapRfcConnection(string connectionString)
        {
            this.connectionString = connectionString;

            if (!RfcDestinationManager.IsDestinationConfigurationRegistered()) 
            {
                RfcDestinationConfig config = new RfcDestinationConfig();
                RfcDestinationManager.RegisterDestinationConfiguration(config);
            }

            this.destination = RfcDestinationManager.GetDestination("NSP");
            this.repository = destination.Repository;
        }

        public void Dispose()
        {
        }

        public RfcResult ExecuteFunction(string functionName)
        {
            return this.ExecuteFunction(functionName, new RfcImportParameter[0]);
        }

        public RfcResult ExecuteFunction(string functionName, RfcImportParameter[] importParameters)
        {
            IRfcFunction function = this.repository.CreateFunction(functionName);
            foreach (var parameter in importParameters)
                function.SetValue(parameter.Name, parameter.Value);

            function.Invoke(destination);
            return new RfcResult(function);
        }
    }
}
