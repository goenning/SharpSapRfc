using SharpSapRfc.Soap.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace SharpSapRfc.Soap
{
    public class SoapRfcWebClient
    {
        private SapSoapRfcDestinationElement destination;
        public SoapRfcWebClient(SapSoapRfcDestinationElement destination)
        {
            this.destination = destination;
        }

        public XmlDocument SendRfcRequest(string functionName, string soapBody)
        {
            HttpWebRequest request = CreateRequest(this.destination.RfcUrl, functionName, "POST");

            string postData = string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:sap-com:document:sap:rfc:functions"">
               <soapenv:Header/>
               <soapenv:Body>
                     {0}
               </soapenv:Body>
            </soapenv:Envelope>", soapBody);

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            using (Stream stream = request.GetRequestStream())
                stream.Write(bytes, 0, bytes.Length);

            return this.ResponseToXml(request);
        }

        public XmlDocument SendWsdlRequest(string functionName)
        {
            HttpWebRequest request = CreateRequest(this.destination.WsdlUrl, functionName, "GET");
            return ResponseToXml(request);
        }

        private XmlDocument ResponseToXml(HttpWebRequest request)
        {
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
                    // this is to prevent that "META start tag" error string
                    // and throw error status "Unathorized" from StatusCode
                    // hence more meaningful
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw ex;
                    }
                    
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                        responseXml.Load(rd);
                }

                if (ex.Status == WebExceptionStatus.Timeout)
                    throw new TimeoutException(string.Format("Timeout on function call. Current timeout is {0} milliseconds.", request.Timeout));

                if (responseXml.InnerXml.Length == 0)
                    throw ex;
            }

            return responseXml;
        }

        private HttpWebRequest CreateRequest(string baseUrl, string functionName, string httpMethod)
        {
            string url = string.Format("{0}?sap-client={1}&services={2}", baseUrl, this.destination.Client, functionName);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "text/xml; charset=\"UTF-8\"";
            request.Accept = "text/xml";
            request.Method = httpMethod;
            request.Timeout = this.destination.Timeout;
            request.KeepAlive = false;
            request.PreAuthenticate = true;
            request.Credentials = new NetworkCredential(this.destination.User, this.destination.Password);
            request.Headers.Add("SOAPAction", "urn:sap-com:document:sap:rfc:functions");
            return request;
        }
    }
}
