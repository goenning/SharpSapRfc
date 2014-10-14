using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Soap.Configuration
{
    public class SapSoapRfcDestinationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SapSoapRfcDestinationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SapSoapRfcDestinationElement)element).Name;
        }

        public IEnumerable<SapSoapRfcDestinationElement> Elements
        {
            get { return this.OfType<SapSoapRfcDestinationElement>(); }
        }
    }
}
