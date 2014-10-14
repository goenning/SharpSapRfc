using System;
using System.Configuration;

namespace SharpSapRfc.Soap.Configuration
{
    public class SapSoapRfcDestinationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"] as string; }
        }

        [ConfigurationProperty("rfcUrl", IsRequired = true)]
        public string RfcUrl
        {
            get { return this["rfcUrl"] as string; }
        }

        [ConfigurationProperty("wsdlUrl", IsRequired = true)]
        public string WsdlUrl
        {
            get { return this["wsdlUrl"] as string; }
        }

        [ConfigurationProperty("client", IsRequired = true)]
        public string Client
        {
            get { return this["client"] as string; }
        }

        [ConfigurationProperty("user", IsRequired = false)]
        public string User
        {
            get { return this["user"] as string; }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get { return this["password"] as string; }
        }
    }
}
