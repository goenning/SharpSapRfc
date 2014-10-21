using System;
using System.Collections.Generic;
using System.Configuration;

namespace SharpSapRfc.Soap.Configuration
{
    public class SapSoapRfcConfigurationSection : ConfigurationSection
    {
        private static object _syncObject = new object();
        private static IDictionary<string, SapSoapRfcDestinationElement> destinationByName = new Dictionary<string, SapSoapRfcDestinationElement>();

        public static SapSoapRfcDestinationElement GetConfiguration(string name)
        {
            if (destinationByName.ContainsKey(name))
                return destinationByName[name];

            lock(_syncObject)
            {
                if (destinationByName.ContainsKey(name))
                    return destinationByName[name];

                SapSoapRfcConfigurationSection section = ConfigurationManager.GetSection("sapSoapRfc") as SapSoapRfcConfigurationSection;
                if (section != null)
                {
                    foreach (SapSoapRfcDestinationElement destination in section.Destinations.Elements)
                    {
                        if (name.Equals(destination.Name))
                        {
                            destinationByName.Add(name, destination);
                            return destinationByName[name];
                        }
                    }
                }
            }


            throw new SharpRfcException(string.Format("Could not find configuration for destination named '{0}' under sapSoapRfc section", name));
        }

        [ConfigurationProperty("destinations")]
        [ConfigurationCollection(typeof(SapSoapRfcDestinationCollection), AddItemName = "add")]
        public SapSoapRfcDestinationCollection Destinations
        {
            get { return (SapSoapRfcDestinationCollection)base["destinations"]; } 
        }
    }
}
