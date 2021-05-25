using System;
using System.Collections.Generic;
using System.Text;

namespace VRP.Data.Models
{
    public class ParametrosEntradaModel
    {
        public int idVRP { get; set; }
        public decimal temperatura { get; set; }
        public decimal pressaoMont { get; set; }
        public decimal pressaoJus { get; set; }
        public decimal vazao { get; set; }
    }
}
