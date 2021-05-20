using System;
using System.Collections.Generic;
using System.Text;

namespace VRP.Data.Models
{
    public class ParametrosVRPModel
    {
        public int idParametro { get; set; }
        public decimal pressao { get; set; }
        public string horaInicial { get; set; }
        public string horaFinal { get; set; }
        public int idVRP { get; set; }
        public bool flStatus { get; set; }
    }
}
