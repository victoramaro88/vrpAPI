using System;
using System.Collections.Generic;
using System.Text;

namespace VRP.Data.Repositories
{
    public class CidadeModel
    {
        public int idCidade { get; set; }
        public string descCidade { get; set; }
        public long codigoIBGE { get; set; }
        public int idEstado { get; set; }
    }
}
