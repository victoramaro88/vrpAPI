using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using VRP.Data.Models;

namespace VRP.Data.Repositories
{
    public class VRPRepository
    {
        private string _scDB_VRP = "";
        private IConfiguration Configuration;

        #region CONSTRUTOR
        public VRPRepository(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            _scDB_VRP = Configuration.GetValue<string>("DB_VRP");
        }
        #endregion

        public List<VRPModel> ListaVRP(int idVRP)
        {
            MySqlDataReader reader = null;
            List<VRPModel> listaRetorno = new List<VRPModel>();

            var query = @"
                            SELECT 
	                            idVRP, descrVRP, modelo, logradouro, 
	                            numero, bairro, cep, latitude, longitude,
	                            imagem, idCidade, idNumCel, status
                            FROM vrp 
                        ";

            if (idVRP > 0)
            {
                query += " WHERE idVRP = @idVRP;";
            }
            else
            {
                query += ";";
            }

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@idVRP", MySqlDbType.Int32);
                com.Parameters["@idVRP"].Value = idVRP;
                con.Open();
                try
                {
                    reader = com.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var ret = new VRPModel()
                            {
                                idVRP = int.Parse(reader["idVRP"].ToString()),
                                descrVRP = reader["descrVRP"].ToString(),
                                modelo = reader["modelo"].ToString(),
                                logradouro = reader["logradouro"].ToString(),
                                numero = reader["numero"].ToString(),
                                bairro = reader["bairro"].ToString(),
                                cep = reader["cep"].ToString(),
                                latitude = decimal.Parse(reader["latitude"].ToString()),
                                longitude = decimal.Parse(reader["longitude"].ToString()),
                                imagem = reader["imagem"].ToString(),
                                idCidade = int.Parse(reader["idCidade"].ToString()),
                                idNumCel = int.Parse(reader["idNumCel"].ToString()),
                                status = reader["status"].ToString() == "1" ? true : false
                            };

                            listaRetorno.Add(ret);
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

                return listaRetorno;
            }
        }

        public List<CidadeModel> ListaCidade(int idCidade)
        {
            MySqlDataReader reader = null;
            List<CidadeModel> listaRetorno = new List<CidadeModel>();

            var query = @"
                            SELECT idCidade, descCidade, codigoIBGE, idEstado
                            FROM cidade 
                        ";

            if (idCidade > 0)
            {
                query += " WHERE idCidade = @idCidade";
            }

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@idCidade", MySqlDbType.Int32);
                com.Parameters["@idCidade"].Value = idCidade;
                con.Open();
                try
                {
                    reader = com.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var ret = new CidadeModel()
                            {
                                idCidade = int.Parse(reader["idCidade"].ToString()),
                                descCidade = reader["descCidade"].ToString(),
                                codigoIBGE = long.Parse(reader["codigoIBGE"].ToString()),
                                idEstado = int.Parse(reader["idEstado"].ToString())
                            };

                            listaRetorno.Add(ret);
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

                return listaRetorno;
            }
        }

        public string InsereHistVRP(HistoricoVRPModel objVRP)
        {
            MySqlDataReader reader = null;
            List<CidadeModel> listaRetorno = new List<CidadeModel>();
            string resp = "";

            var query = @"
                            INSERT INTO historicovrp
                            (temperatura, pressaoMont, pressaoJus, vazao, dataHora, idVRP)
                            VALUES
                            (
                                @temperatura, @pressaoMont, @pressaoJus, 
                                @vazao, (SELECT NOW()), @idVRP
                            );
                            SELECT 'OK' AS Retorno;
                        ";

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@temperatura", MySqlDbType.Decimal);
                com.Parameters["@temperatura"].Value = objVRP.temperatura;
                com.Parameters.Add("@pressaoMont", MySqlDbType.Decimal);
                com.Parameters["@pressaoMont"].Value = objVRP.pressaoMont;
                com.Parameters.Add("@pressaoJus", MySqlDbType.Decimal);
                com.Parameters["@pressaoJus"].Value = objVRP.pressaoJus;
                com.Parameters.Add("@vazao", MySqlDbType.Decimal);
                com.Parameters["@vazao"].Value = objVRP.vazao;
                com.Parameters.Add("@idVRP", MySqlDbType.Int32);
                com.Parameters["@idVRP"].Value = objVRP.idVRP;
                con.Open();
                try
                {
                    reader = com.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                        resp = reader["Retorno"].ToString();
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

                return resp;
            }
        }


        public List<ParametrosVRPModel> ListaParametrosVRP(int idVRP)
        {
            MySqlDataReader reader = null;
            List<ParametrosVRPModel> listaRetorno = new List<ParametrosVRPModel>();

            var query = @"
                            SELECT
	                            idParametro, pressao, horaInicial, horaFinal,
	                            idVRP, flStatus
                            FROM parametrosvrp
                            WHERE idVRP = @idVRP and flStatus = 1;
                        ";

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@idVRP", MySqlDbType.Int32);
                com.Parameters["@idVRP"].Value = idVRP;
                con.Open();
                try
                {
                    reader = com.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var ret = new ParametrosVRPModel()
                            {
                                idParametro = int.Parse(reader["idParametro"].ToString()),
                                pressao = decimal.Parse(reader["pressao"].ToString()),
                                horaInicial = reader["horaInicial"].ToString(),
                                horaFinal = reader["horaFinal"].ToString(),
                                idVRP = int.Parse(reader["idVRP"].ToString()),
                                flStatus = reader["flStatus"].ToString() == "1" ? true : false
                            };

                            listaRetorno.Add(ret);
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

                return listaRetorno;
            }
        }
    }
}
