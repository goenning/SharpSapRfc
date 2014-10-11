using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace SharpSapRfc.Soap
{
    public class SoapRfcPreparedFunction : RfcPreparedFunction
    {
        private string functionName;

        public SoapRfcPreparedFunction(string functionName)
        {
            this.functionName = functionName;
        }

        public override RfcResult Execute()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://sap-vm:8000/sap/bc/soap/rfc?sap-client=001&services=" + this.functionName);
            request.ContentType = "text/xml;charset=\"utf-8\"";
            request.Accept = "text/xml";
            request.Method = "POST";
            request.Credentials = new NetworkCredential("bcuser", "sapadmin2");

            StringBuilder parametersXml = new StringBuilder();
            foreach (var parameter in this.parameters)
                parametersXml.AppendFormat("<{0}>{1}</{0}>", parameter.Name.ToUpper(), parameter.Value);

            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:sap-com:document:sap:rfc:functions"">
               <soapenv:Header/>
               <soapenv:Body>
                  <urn:{0}>
                     {1}
                  </urn:{0}>
               </soapenv:Body>
            </soapenv:Envelope>", this.functionName, parametersXml.ToString()));

            using (Stream stream = request.GetRequestStream())
                soapEnvelopeXml.Save(stream);

            XmlDocument responseXml = new XmlDocument();
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                        responseXml.Load(rd);
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                        responseXml.Load(rd);
                }
            }

            var responseTag = responseXml.GetElementsByTagName(string.Format("urn:{0}.Response", this.functionName));
            if (responseTag.Count > 0)
                return new SoapRfcResult(responseTag[0]);

            var exceptionTag = responseXml.GetElementsByTagName(string.Format("rfc:{0}.Exception", this.functionName));
            if (exceptionTag.Count > 0)
                throw new RfcException(exceptionTag[0].InnerText);

            throw new Exception("Could not fetch response tag.");
        }
    }
}
