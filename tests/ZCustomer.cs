
namespace SharpSapRfc.Test
{
    public class ZCustomer
    {
        [RfcStructureField("ID")]
        public int Id { get; set; }

        [RfcStructureField("NAME")]
        public string Name { get; set; }

        [RfcStructureField("ACTIVE")]
        public bool IsActive { get; set; }

        public int Age { get; set; }
    }
}
