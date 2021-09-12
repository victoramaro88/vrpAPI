using System;
using System.Collections.Generic;
using System.Text;

namespace PC.Data.Models
{
    public class ParamListaHistoricoPCModel
    {
        public int idPC { get; set; }
        public DateTime? dataInicial { get; set; }
        public DateTime? dataFinal { get; set; }
        public int linhas { get; set; }
    }
}
