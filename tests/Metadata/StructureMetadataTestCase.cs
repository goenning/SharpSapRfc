using SharpSapRfc.Metadata;
using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using SharpSapRfc.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpSapRfc.Test.Metadata
{
    public class Soap_StructureMetadataTestCase : StructureMetadataTestCase
    {
        private SoapSapRfcConnection conn;

        public override RfcMetadataCache GetMetadataCache()
        {
            this.conn = new SoapSapRfcConnection("TST-SOAP");
            SoapRfcWebClient webClient = new SoapRfcWebClient(this.conn.Destination);
            return new SoapRfcMetadataCache(webClient);
        }

        public override void Dispose()
        {
            this.conn.Dispose();
        }
    }

    public class Plain_StructureMetadataTestCase : StructureMetadataTestCase
    {
        private PlainSapRfcConnection conn;

        public override RfcMetadataCache GetMetadataCache()
        {
            this.conn = new PlainSapRfcConnection("TST");
            return new PlainRfcMetadataCache(this.conn);
        }

        public override void Dispose()
        {
            this.conn.Dispose();
        }
    }

    public abstract class StructureMetadataTestCase : IDisposable
    {
        public abstract RfcMetadataCache GetMetadataCache();

        [Fact]
        public void InputStructureMetadataTest()
        {
            var cache = GetMetadataCache();
            var metadata = cache.GetFunctionMetadata("Z_SSRT_IN_OUT");

            var parameter = metadata.GetInputParameter("I_MARA");
            Assert.NotNull(parameter.StructureMetadata);

            AssertStructureMetadataField(parameter.StructureMetadata, "MANDT", AbapDataType.CHAR);
            AssertStructureMetadataField(parameter.StructureMetadata, "ID", AbapDataType.INTEGER);
            AssertStructureMetadataField(parameter.StructureMetadata, "NAME", AbapDataType.CHAR);
            AssertStructureMetadataField(parameter.StructureMetadata, "PRICE", AbapDataType.DECIMAL);
            AssertStructureMetadataField(parameter.StructureMetadata, "DATUM", AbapDataType.DATE);
            AssertStructureMetadataField(parameter.StructureMetadata, "UZEIT", AbapDataType.TIME);
            AssertStructureMetadataField(parameter.StructureMetadata, "ACTIVE", AbapDataType.CHAR);
            AssertStructureMetadataField(parameter.StructureMetadata, "STATE", AbapDataType.SHORT);
        }

        private void AssertStructureMetadataField(StructureMetadata metadata, string name, AbapDataType dataType)
        {
            var field = metadata.GetField(name);
            Assert.Equal(name, field.Name);
            Assert.Equal(dataType, field.DataType);
        }

        public abstract void Dispose();
    }
}
