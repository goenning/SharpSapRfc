
namespace SharpSapRfc.Test.Model
{
    public class RepositoryObject
    {
        [RfcStructureField("OBJ_NAME")]
        public string Name { get; set; }
        [RfcStructureField("DELFLAG")]
        public bool DeletionFlag { get; set; }
    }
}