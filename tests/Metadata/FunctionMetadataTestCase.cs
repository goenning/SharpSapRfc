using SharpSapRfc.Metadata;
using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using SharpSapRfc.Soap.Configuration;
using SharpSapRfc.Types;
using System;
using Xunit;
using System.Linq;

namespace SharpSapRfc.Test.Metadata
{
    public class Soap_FunctionMetadataTestCase : FunctionMetadataTestCase
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

    public class Plain_FunctionMetadataTestCase : FunctionMetadataTestCase
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

    public abstract class FunctionMetadataTestCase : IDisposable
    {
        public abstract RfcMetadataCache GetMetadataCache();

        [Fact]
        public void SimpleFunctionMetadataTest()
        {
            var cache = GetMetadataCache();
            var metadata = cache.GetFunctionMetadata("Z_SSRT_SUM");

            Assert.Equal(2, metadata.InputParameters.Length);
            Assert.Equal(1, metadata.OutputParameters.Length);
            Assert.Equal("Z_SSRT_SUM", metadata.Name);

            AssertInputParameter(metadata, "I_NUM1", AbapDataType.INTEGER);
            AssertInputParameter(metadata, "I_NUM2", AbapDataType.INTEGER);
            AssertOutputParameter(metadata, "E_RESULT", AbapDataType.INTEGER);
        }

        [Fact]
        public void GetStructureMetadataDetailsTest()
        {
            var cache = GetMetadataCache();
            var metadata = cache.GetFunctionMetadata("Z_SSRT_GET_ORDER");
            var orderMetadata = metadata.GetOutputParameter("E_ORDER");

            Assert.Equal(orderMetadata.DataType, AbapDataType.STRUCTURE);
            Assert.Equal(orderMetadata.StructureMetadata.Name, "ZSTR_SSRT_ORDER");
            Assert.Equal(orderMetadata.StructureMetadata.Fields.Count(), 2);

            var idField = orderMetadata.StructureMetadata.GetField("ID");
            Assert.Equal(idField.DataType, AbapDataType.INTEGER);

            var itemsField = orderMetadata.StructureMetadata.GetField("ITEMS");
            Assert.Equal(itemsField.DataType, AbapDataType.TABLE);
        }

        [Fact]
        public void InOutFunctionMetadataTest()
        {
            var cache = GetMetadataCache();
            var metadata = cache.GetFunctionMetadata("Z_SSRT_IN_OUT");
           
            Assert.Equal(13, metadata.InputParameters.Length);
            Assert.Equal(15, metadata.OutputParameters.Length);
            Assert.Equal("Z_SSRT_IN_OUT", metadata.Name);

            AssertInputParameter(metadata, "I_ID", AbapDataType.INTEGER);
            AssertInputParameter(metadata, "I_INT1", AbapDataType.BYTE);
            AssertInputParameter(metadata, "I_FLOAT", AbapDataType.DOUBLE);
            AssertInputParameter(metadata, "I_DEC_3_2", AbapDataType.DECIMAL);
            AssertInputParameter(metadata, "I_DEC_23_4", AbapDataType.DECIMAL);
            AssertInputParameter(metadata, "I_DEC_30_7", AbapDataType.DECIMAL);
            AssertInputParameter(metadata, "I_DATUM", AbapDataType.DATE);
            AssertInputParameter(metadata, "I_UZEIT", AbapDataType.TIME);
            AssertInputParameter(metadata, "I_ACTIVE", AbapDataType.CHAR);
            AssertInputParameter(metadata, "I_MARA", AbapDataType.STRUCTURE);
            AssertInputParameter(metadata, "I_MULTIPLE_ID", AbapDataType.TABLE);
            AssertInputParameter(metadata, "I_MULTIPLE_NAME", AbapDataType.TABLE);

            AssertOutputParameter(metadata, "E_ID", AbapDataType.INTEGER);
            AssertOutputParameter(metadata, "E_INT1", AbapDataType.BYTE);
            AssertOutputParameter(metadata, "E_FLOAT", AbapDataType.DOUBLE);
            AssertOutputParameter(metadata, "E_DEC_3_2", AbapDataType.DECIMAL);
            AssertOutputParameter(metadata, "E_DEC_23_4", AbapDataType.DECIMAL);
            AssertOutputParameter(metadata, "E_DEC_30_7", AbapDataType.DECIMAL);
            AssertOutputParameter(metadata, "E_DATUM", AbapDataType.DATE);
            AssertOutputParameter(metadata, "E_UZEIT", AbapDataType.TIME);
            AssertOutputParameter(metadata, "E_ACTIVE", AbapDataType.CHAR);
            AssertOutputParameter(metadata, "E_MARA_DATUM", AbapDataType.DATE);
            AssertOutputParameter(metadata, "E_MARA_UZEIT", AbapDataType.TIME);
            AssertOutputParameter(metadata, "E_MARA_ID", AbapDataType.INTEGER);
            AssertOutputParameter(metadata, "E_MULTIPLE_ID", AbapDataType.TABLE);
            AssertOutputParameter(metadata, "E_MULTIPLE_NAME", AbapDataType.TABLE);
        }

        private void AssertInputParameter(FunctionMetadata metadata, string name, AbapDataType dataType)
        {
            var parameter = metadata.GetInputParameter(name);
            Assert.Equal(name, parameter.Name);
            Assert.Equal(dataType, parameter.DataType);
        }

        private void AssertOutputParameter(FunctionMetadata metadata,  string name, AbapDataType dataType)
        {
            var parameter = metadata.GetOutputParameter(name);
            Assert.Equal(name, parameter.Name);
            Assert.Equal(dataType, parameter.DataType);
        }

        public abstract void Dispose();
    }
}
