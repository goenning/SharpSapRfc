using SAP.Middleware.Connector;
using System;

namespace SharpSapRfc.Plain
{
    public class PlainSapRfcConnection : SapRfcConnection
    {
        public RfcRepository Repository 
        {
            get {
                this.EnsureConnectionIsOpen();
                return this.repository;
            }
        }

        public RfcDestination Destination
        {
            get
            {
                this.EnsureConnectionIsOpen();
                return this.destination;
            }
        }

        public PlainSapRfcConnection(string destinationName)
        {
            this.destinationName = destinationName;
            this.structureMapper = new PlainRfcStructureMapper(new PlainRfcValueMapper());
        }

        private void EnsureConnectionIsOpen()
        {
            if (!isOpen)
            {
                try { 
                    this.destination = RfcDestinationManager.GetDestination(destinationName);
                    this.repository = this.destination.Repository;
                    this.isOpen = true;
                }
                catch (Exception ex)
                {
                    throw new SharpRfcCallException("Could not connect to SAP.", ex);
                }
            }
        }

        public override RfcPreparedFunction PrepareFunction(string functionName)
        {
            EnsureConnectionIsOpen();
            return new PlainRfcPreparedFunction(functionName, this.structureMapper, this.Repository, this.Destination);
        }

        public override void Dispose()
        {
            
        }

        private string destinationName;
        private bool isOpen = false;
        private RfcRepository repository;
        private RfcDestination destination;
        private PlainRfcStructureMapper structureMapper;

        public override RfcStructureMapper GetStructureMapper()
        {
            return this.structureMapper;
        }

        public override void SetTimeout(int timeout)
        {
            //there is no timeout for plain rfc
        }
    }
}
