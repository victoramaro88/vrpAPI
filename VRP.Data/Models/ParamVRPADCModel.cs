using System;
using System.Collections.Generic;
using System.Text;

namespace VRP.Data.Models
{
    public class ParamVRPADCModel
    {
        public int idVRP { get; set; }
        public int tempoEnvioMinutos { get; set; }
        public int fatorMultVaz { get; set; }
        public decimal pressao { get; set; }
        public bool flStatusADC { get; set; }
        public DateTime dataUltimoRegistro { get; set; }
    }
}
