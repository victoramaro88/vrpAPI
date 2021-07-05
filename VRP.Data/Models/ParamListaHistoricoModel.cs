using System;
using System.Collections.Generic;
using System.Text;

namespace VRP.Data.Models
{
    public class ParamListaHistoricoModel
    {
        public int idVRP { get; set; }
        public DateTime? dataInicial { get; set; }
        public DateTime? dataFinal { get; set; }
        public int linhas { get; set; }
    }
}
