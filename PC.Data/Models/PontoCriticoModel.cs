using System;
using System.Collections.Generic;
using System.Text;

namespace PC.Data.Models
{
    public class PontoCriticoModel
    {
        public int idPC { get; set; }
        public string descPC { get; set; }
        public string modeloPC { get; set; }
        public string logradouroPC { get; set; }
        public string numeroPC { get; set; }
        public string bairroPC { get; set; }
        public string cepPC { get; set; }
        public decimal latitudePC { get; set; }
        public decimal longitudePC { get; set; }
        public string imagemPC { get; set; }
        public int idCidade { get; set; }
        public string descCidade { get; set; }
        public int idNumCel { get; set; }
        public int tempoEnvioMinutos { get; set; }
        public int fatorMultVaz { get; set; }
        public bool statusVazao { get; set; }
        public bool statusPC { get; set; }
    }
}
