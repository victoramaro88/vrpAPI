using System;
using System.Collections.Generic;
using System.Text;

namespace VRP.Data.Models
{
    public class NumeroCelularModel
    {
        public int idNumCel { get; set; }
        public string ddi { get; set; }
        public string ddd { get; set; }
        public string numero { get; set; }
        public int idOperadora { get; set; }
        public bool status { get; set; }
    }
}
