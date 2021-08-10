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
        public List<UsuarioModel> BuscarUsuario(string cpf)
        {
            MySqlDataReader reader = null;
            List<UsuarioModel> listaUsuario = new List<UsuarioModel>();

            var query = @"
                            SELECT * FROM vrp_horninksys.usuario
                        ";

            if (cpf.Length > 0)
            {
                query += " WHERE cpfUsuario = @cpf";
            }
            else
            {
                query += ";";
            }

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@cpf", MySqlDbType.VarChar);
                com.Parameters["@cpf"].Value = cpf;
                con.Open();
                try
                {
                    reader = com.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var objUsuario = new UsuarioModel()
                            {
                                idUsuario = int.Parse(reader["idUsuario"].ToString()),
                                cpfUsuario = reader["cpfUsuario"].ToString(),
                                nomeUsuario = reader["nomeUsuario"].ToString(),
                                statusUsuario = (reader["statusUsuario"].ToString() == "1" ? true : false),
                                idPerfil = int.Parse(reader["idPerfil"].ToString())
                            };

                            listaUsuario.Add(objUsuario);
                        }
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

                return listaUsuario;
            }
        }
        public List<PerfilUsuarioModel> BuscarPerfil(int idPerfil)
        {
            MySqlDataReader reader = null;
            List<PerfilUsuarioModel> listaPerfil = new List<PerfilUsuarioModel>();

            var query = @"
                            SELECT * FROM vrp_horninksys.perfilusuario
                            WHERE statusPerfil = 1
                        ";

            if (idPerfil > 0)
            {
                query += " AND idPerfil = @idPerfil";
            }
            else
            {
                query += ";";
            }

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@idPerfil", MySqlDbType.Int32);
                com.Parameters["@idPerfil"].Value = idPerfil;
                con.Open();
                try
                {
                    reader = com.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var objPerfil = new PerfilUsuarioModel()
                            {
                                idPerfil = int.Parse(reader["idPerfil"].ToString()),
                                descPerfil = reader["descPerfil"].ToString(),
                                statusPerfil = (reader["statusPerfil"].ToString() == "1" ? true : false),
                            };

                            listaPerfil.Add(objPerfil);
                        }
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

                return listaPerfil;
            }
        }

        public string ManterUsuario()
        {
            return "";
        }
    }
}
