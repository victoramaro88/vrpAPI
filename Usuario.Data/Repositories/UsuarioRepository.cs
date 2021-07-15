using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;
using Usuario.Data.Models;

namespace Usuario.Data.Repositories
{
    public class UsuarioRepository
    {
        private string _scDB_VRP = "";
        private IConfiguration Configuration;

        #region CONSTRUTOR
        public UsuarioRepository(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            _scDB_VRP = Configuration.GetValue<string>("DB_VRP");
        }
        #endregion


        public UsuarioModel LoginUsuario(string cpf, string senha)
        {
            MySqlDataReader reader = null;
            UsuarioModel objUsuario = new UsuarioModel();

            var query = @"
                            SELECT * FROM vrp_horninksys.usuario 
                            WHERE cpfUsuario = @cpf 
                            AND senhaUsuario = @senha
                        ";

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@cpf", MySqlDbType.VarChar);
                com.Parameters["@cpf"].Value = cpf;
                com.Parameters.Add("@senha", MySqlDbType.VarChar);
                com.Parameters["@senha"].Value = senha;
                con.Open();
                try
                {
                    reader = com.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                        objUsuario.idUsuario = int.Parse(reader["idUsuario"].ToString());
                        objUsuario.cpfUsuario = reader["cpfUsuario"].ToString();
                        objUsuario.nomeUsuario = reader["nomeUsuario"].ToString();
                        objUsuario.statusUsuario = (reader["statusUsuario"].ToString() == "1" ? true : false);
                        objUsuario.idPerfil = int.Parse(reader["idPerfil"].ToString());
                    }
                }

                catch (Exception e)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }

                return objUsuario;
            }
        }

    }
}
