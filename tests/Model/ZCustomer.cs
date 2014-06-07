
namespace SharpSapRfc.Test.Model
{
    public class ZCustomer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [RfcStructureField("ACTIVE")]
        public bool IsActive { get; set; }

        public int Age { get; set; }
    }
}
