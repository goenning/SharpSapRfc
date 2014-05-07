
namespace SharpSapRfc.Test
{
    public class ZCliente
    {
        [RfcStructureField("ID")]
        public int Codigo { get; set; }
        [RfcStructureField("NOME")]
        public string Nome { get; set; }

        public int Idade { get; set; }
    }
}
