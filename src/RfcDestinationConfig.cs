using SAP.Middleware.Connector;

namespace SharpSapRfc
{
    public class RfcDestinationConfig : IDestinationConfiguration
    {
        public RfcConfigParameters GetParameters(string destinationName)
        {
            RfcConfigParameters conf = new RfcConfigParameters();
            if (destinationName == "NSP")
            {
                conf.Add(RfcConfigParameters.AppServerHost, "sap-vm");
                conf.Add(RfcConfigParameters.SystemNumber, "00");
                conf.Add(RfcConfigParameters.SystemID, "xxx");
                conf.Add(RfcConfigParameters.User, "bcuser");
                conf.Add(RfcConfigParameters.Password, "sapadmin2");
                conf.Add(RfcConfigParameters.Client, "001");
            }
            return conf;
        }

        public bool ChangeEventsSupported()
        {
            return true;
        }

		public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged; 
    }
}
