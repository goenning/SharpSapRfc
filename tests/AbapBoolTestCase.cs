using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAP.Middleware.Connector;
using System;

namespace SharpSapRfc.Test
{
    [TestClass]
    public class AbapBoolTestCase
    {
        [TestMethod]
        public void XToTrueTest()
        {
            Boolean boolean = AbapBool.FromString("X");
            Assert.AreEqual(true, boolean);
        }

        [TestMethod]
        public void SpaceToFalseTest()
        {
            Boolean boolean = AbapBool.FromString(" ");
            Assert.AreEqual(false, boolean);
        }

        [TestMethod]
        public void EmptyToFalseTest()
        {
            Boolean boolean = AbapBool.FromString("");
            Assert.AreEqual(false, boolean);
        }

        [TestMethod]
        [ExpectedException(typeof(RfcAbapException))]
        public void UnknowToExceptionTest()
        {
            AbapBool.FromString("A");
        }
    }
}
