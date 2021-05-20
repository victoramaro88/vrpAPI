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
                            VALUES
                            (1, 'Daniel', 'Barret'); 
                        ";

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@temperatura", MySqlDbType.Decimal);
                com.Parameters["@temperatura"].Value = objVRP.temperatura;
                //-> CONTINUAR COM OS DEMAIS PARÂMETROS.***********
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
    }
}
