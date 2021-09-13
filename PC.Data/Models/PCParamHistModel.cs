using System;
using System.Collections.Generic;
using System.Text;

namespace PC.Data.Models
{
    public class PCParamHistModel
    {
        public int idPC { get; set; }
        public int tempoEnvioMinutos { get; set; }
        public int fatorMultVaz { get; set; }
        public bool statusVazao { get; set; }
        public DateTime dataUltimoRegistro { get; set; }
    }
}
