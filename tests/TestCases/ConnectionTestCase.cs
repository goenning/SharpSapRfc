using SAP.Middleware.Connector;
using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using SharpSapRfc.Soap.Configuration;
using SharpSapRfc.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpSapRfc.Test.TestCases
{
    public class Soap_ConnectionTestCase : ConnectionTestCase
    {
        protected override SapRfcConnection GetConnection()
        {
            SapSoapRfcDestinationElement cfg = new SapSoapRfcDestinationElement();
            cfg.Client = "001";
            cfg.Name = "CUSTOM-SOAP-CONN";
            cfg.RfcUrl = "http://sap-vm:8000/sap/bc/soap/rfc";
            cfg.WsdlUrl = "http://sap-vm:8000/sap/bc/soap/wsdl";
            cfg.User = "rfc_super";
            cfg.Password = "rfcsuper1";
            cfg.Timeout = 5000;
            return new SoapSapRfcConnection(cfg);
        }
    }

    public class Plain_ConnectionTestCase : ConnectionTestCase, IDisposable
    {
        public class CustonDestinationConfiguration : IDestinationConfiguration 
        {
            public bool ChangeEventsSupported()
            {
                return false;
            }

            public RfcConfigParameters GetParameters(string destinationName)
            {
                if (destinationName == "CUSTOM-PLAIN-CONN")
                {
                    var cfg = new RfcConfigParameters();
                    cfg.Add(RfcConfigParameters.Name, "CUSTOM-PLAIN-CONN");
                    cfg.Add(RfcConfigParameters.User, "rfc_super");
                    cfg.Add(RfcConfigParameters.Password, "rfcsuper1");
                    cfg.Add(RfcConfigParameters.Client, "001");
                    cfg.Add(RfcConfigParameters.SystemNumber, "00");
                    cfg.Add(RfcConfigParameters.AppServerHost, "sap-vm");
                    cfg.Add(RfcConfigParameters.Language, "EN");
                    return cfg;
                }
                return null;
            }

            public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;
        }

        private CustonDestinationConfiguration cfg;
        public Plain_ConnectionTestCase()
        {
            this.cfg = new CustonDestinationConfiguration();
        }

        protected override SapRfcConnection GetConnection()
        {
            RfcDestinationManager.RegisterDestinationConfiguration(this.cfg);
            return new PlainSapRfcConnection("CUSTOM-PLAIN-CONN");
        }

        public void Dispose()
        {
            RfcDestinationManager.UnregisterDestinationConfiguration(this.cfg);
        }
    }

    public abstract class ConnectionTestCase
    {
        protected abstract SapRfcConnection GetConnection();

        [Fact]
        public void ObjectFunctionTest()
        {
            using (SapRfcConnection conn = GetConnection())
            {
                var scarr = conn.ReadTable<AirlineCompany>("SCARR");
                Assert.NotEqual(0, scarr.Count());
            }
        }
    }
}
