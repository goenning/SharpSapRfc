using SharpSapRfc.Soap.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpSapRfc.Test.TestCases
{
    public class SoapConfigurationSectionTestCase
    {
        [Fact]
        public void ParametersCanBeRead()
        {
            var destination = SapSoapRfcConfigurationSection.GetConfiguration("TST-SOAP");
            Assert.Equal("TST-SOAP", destination.Name);
            Assert.Equal("http://sap-vm:8000/sap/bc/soap/rfc", destination.RfcUrl);
            Assert.Equal("http://sap-vm:8000/sap/bc/soap/wsdl", destination.WsdlUrl);
            Assert.Equal("001", destination.Client);
            Assert.Equal("rfc_super", destination.User);
            Assert.Equal("rfcsuper1", destination.Password);
            Assert.Equal(5000, destination.Timeout);
        }
    }
}
