using System;
using System.Collections.Generic;
using System.Text;

namespace VRP.Data.Models
{
    public class ParametrosIAModel
    {
        public decimal volume_1 { get; set; } //-> Ultimo volume acumulado atual
        public decimal pressão_jusante { get; set; } //-> pressão em mca na saida da VRP
        public decimal vazão_instântanea_1 { get; set; } //-> litros por segundo de vazão instantanea
        public decimal PC { get; set; } //-> pressão em mca no ponto critico referente a VRP
        public string hora { get; set; } //-> hora e minuto da requisição
        public int dia_semana { get; set; } //-> dia da semana da requisição(0 - segunda, 6 - domingo)
        public string abertura { get; set; } //-> hora e minuto que se deseja abrir a vrp
        public string fechamento { get; set; } //-> hora e minuto que se deseja fechar a vrp
    }
}
