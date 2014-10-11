using SharpSapRfc.Metadata;
using SharpSapRfc.Plain;
using SharpSapRfc.Soap;
using SharpSapRfc.Types;
using System;
using Xunit;

namespace SharpSapRfc.Test.Metadata
{
    public class Soap_FunctionMetadataTestCase : FunctionMetadataTestCase
    {
        private SapSoapRfcConnection conn;

        public override AbapMetadataCache GetMetadataCache()
        {
            this.conn = new SapSoapRfcConnection();
            return new SoapAbapMetadataCache();
        }

        public override void Dispose()
        {
            this.conn.Dispose();
        }
    }

    public class Plain_FunctionMetadataTestCase : FunctionMetadataTestCase
    {
        private SapPlainRfcConnection conn;

        public override AbapMetadataCache GetMetadataCache()
        {
            this.conn = new SapPlainRfcConnection("TST");
            return new PlainAbapMetadataCache(this.conn);
        }

        public override void Dispose()
        {
            this.conn.Dispose();
        }
    }

    public abstract class FunctionMetadataTestCase : IDisposable
    {
        public abstract AbapMetadataCache GetMetadataCache();

        [Fact]
        public void SimpleFunctionMetadataTest()
        {
            var cache = GetMetadataCache();
            var metadata = cache.GetFunctionMetadata("Z_SSRT_SUM");
            Assert.Equal(2, metadata.ImportParameters.Length);
            Assert.Equal(1, metadata.ExportParameters.Length);

            Assert.Equal("Z_SSRT_SUM", metadata.FunctionName);

            ParameterMetadata parameter = null;

            parameter = metadata.GetImportParameter(0);
            Assert.Equal("I_NUM1", parameter.Name);
            Assert.Equal(AbapDataType.INTEGER, parameter.DataType);

            parameter = metadata.GetImportParameter(1);
            Assert.Equal("I_NUM2", parameter.Name);
            Assert.Equal(AbapDataType.INTEGER, parameter.DataType);

            parameter = metadata.GetExportParameter(0);
            Assert.Equal("E_RESULT", parameter.Name);
            Assert.Equal(AbapDataType.INTEGER, parameter.DataType);
        }

        public abstract void Dispose();
    }
}
