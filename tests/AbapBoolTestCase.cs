using Xunit;
using SAP.Middleware.Connector;
using System;

namespace SharpSapRfc.Test
{
    public class AbapBoolTestCase
    {
        [Fact]
        public void XToTrueTest()
        {
            Boolean boolean = AbapBool.FromString("X");
            Assert.Equal(true, boolean);
        }

        [Fact]
        public void TrueToXTest()
        {
            string X = AbapBool.ToString(true);
            Assert.Equal("X", X);
        }

        [Fact]
        public void SpaceToFalseTest()
        {
            Boolean boolean = AbapBool.FromString(" ");
            Assert.Equal(false, boolean);
        }

        [Fact]
        public void FalseToSpaceTest()
        {
            string X = AbapBool.ToString(false);
            Assert.Equal(" ", X);
        }

        [Fact]
        public void EmptyToFalseTest()
        {
            Boolean boolean = AbapBool.FromString("");
            Assert.Equal(false, boolean);
        }

        [Fact]
        public void UnknowToExceptionTest()
        {
            Assert.Throws(typeof(RfcMappingException), () =>
            {
                AbapBool.FromString("A");
            });
        }
    }
}
