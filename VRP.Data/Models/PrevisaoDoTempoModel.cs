using System;
using System.Collections.Generic;
using System.Text;

namespace VRP.Data.Models
{
    public class PrevisaoDoTempoModel
    {
        public string id_prevtemp { get; set; }
        public DateTime data_hora { get; set; }
        public decimal chuva { get; set; }
        public decimal temperatura { get; set; }
    }
}
