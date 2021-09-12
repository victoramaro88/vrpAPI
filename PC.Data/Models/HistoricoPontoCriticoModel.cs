using System;
using System.Collections.Generic;
using System.Text;

namespace PC.Data.Models
{
    public class HistoricoPontoCriticoModel
    {
        public int idHisPC { get; set; }
        public int idPC { get; set; }
        public decimal pressaoPC { get; set; }
        public decimal vazaoPC { get; set; }
        public decimal tensaoBateriaPC { get; set; }
        public DateTime dataHoraPC { get; set; }
    }
}
