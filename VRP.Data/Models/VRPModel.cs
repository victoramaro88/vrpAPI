using System;
using System.Collections.Generic;
using System.Text;

namespace VRP.Data.Models
{
    public class VRPModel
    {
        public int idVRP { get; set; }
        public string descrVRP { get; set; }
        public string modelo { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public string imagem { get; set; }
        public int idCidade { get; set; }
        public int idNumCel { get; set; }
        public bool status { get; set; }
    }
}
