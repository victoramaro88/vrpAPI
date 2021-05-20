using System;
using System.Collections.Generic;
using System.Text;

namespace VRP.Data.Models
{
    public class HistoricoVRPModel
    {
        public int idHistorico { get; set; }
        public decimal temperatura { get; set; }
        public decimal pressaoMont { get; set; }
        public decimal pressaoJus { get; set; }
        public decimal vazao { get; set; }
        public DateTime dataHora { get; set; }
        public int idVRP { get; set; }
    }
}
