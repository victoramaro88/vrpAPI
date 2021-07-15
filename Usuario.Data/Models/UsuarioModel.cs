using System;
using System.Collections.Generic;
using System.Text;

namespace Usuario.Data.Models
{
    public class UsuarioModel
    {
        public int idUsuario { get; set; }
        public string cpfUsuario { get; set; }
        public string nomeUsuario { get; set; }
        public string senhaUsuario { get; set; }
        public int idPerfil { get; set; }
        public bool statusUsuario { get; set; }
        public string erroMensagem { get; set; }
    }
}
