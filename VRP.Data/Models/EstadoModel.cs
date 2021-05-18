using System;
using System.Collections.Generic;
using System.Text;

namespace VRP.Data.Repositories
{
    public class EstadoModel
    {
        public int idEstado { get; set; }
        public string descEstado { get; set; }
        public string sigla { get; set; }
        public int codigoIBGE { get; set; }
    }
}
