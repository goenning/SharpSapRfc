using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSapRfc.Test.Model
{
    class RepositoryObject
    {
        [RfcStructureField("OBJ_NAME")]
        public string Name { get; set; }
        [RfcStructureField("DELFLAG")]
        public bool DeletionFlag { get; set; }
    }
}
