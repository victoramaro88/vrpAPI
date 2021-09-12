using Microsoft.Extensions.Configuration;
using MySqlConnector;
using PC.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PC.Data.Repositories
{
    public class PontoCriticoRepository
    {
        private string _scDB_VRP = "";
        private IConfiguration Configuration;

        #region CONSTRUTOR
        public PontoCriticoRepository(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            _scDB_VRP = Configuration.GetValue<string>("DB_VRP");
        }
        #endregion

        public List<PontoCriticoModel> ListaPontoCritico(int idPC)
        {
            MySqlDataReader reader = null;
            List<PontoCriticoModel> listaRetorno = new List<PontoCriticoModel>();

            var query = @"
                            SELECT 
	                            PC.idPC, PC.descrPC, PC.modeloPC, PC.logradouroPC, PC.numeroPC, PC.bairroPC
                                , PC.cepPC, PC.latitudePC, PC.longitudePC, PC.imagemPC, PC.idCidade, CIDADE.descCidade
                                , PC.idNumCel, PC.tempoEnvioMinutos, PC.fatorMultVaz, PC.statusVazao, PC.statusPC
                            FROM vrp_horninksys.pontocritico AS PC
                            INNER JOIN vrp_horninksys.cidade AS CIDADE ON CIDADE.idCidade = PC.idCidade
                        ";

            if (idPC > 0)
            {
                query += " WHERE PC.idPC = @idPC;";
            }
            else
            {
                query += ";";
            }

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@idPC", MySqlDbType.Int32);
                com.Parameters["@idPC"].Value = idPC;
                con.Open();
                try
                {
                    reader = com.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var ret = new PontoCriticoModel()
                            {
                                idPC = int.Parse(reader["idPC"].ToString()),
                                descPC = reader["descPC"].ToString(),
                                modeloPC = reader["modeloPC"].ToString(),
                                logradouroPC = reader["logradouroPC"].ToString(),
                                numeroPC = reader["numeroPC"].ToString(),
                                bairroPC = reader["bairroPC"].ToString(),
                                cepPC = reader["cepPC"].ToString(),
                                latitudePC = decimal.Parse(reader["latitudePC"].ToString()),
                                longitudePC = decimal.Parse(reader["longitudePC"].ToString()),
                                imagemPC = reader["imagemPC"].ToString(),
                                idCidade = int.Parse(reader["idCidade"].ToString()),
                                descCidade = reader["descCidade"].ToString(),
                                idNumCel = int.Parse(reader["idNumCel"].ToString()),
                                tempoEnvioMinutos = int.Parse(reader["tempoEnvioMinutos"].ToString()),
                                fatorMultVaz = int.Parse(reader["fatorMultVaz"].ToString()),
                                statusVazao = reader["statusVazao"].ToString() == "1" ? true : false,
                                statusPC = reader["statusPC"].ToString() == "1" ? true : false
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

        public List<PontoCriticoModel> ListaPontoCriticoByIdVRP(int idVRP)
        {
            MySqlDataReader reader = null;
            List<PontoCriticoModel> listaRetorno = new List<PontoCriticoModel>();

            var query = @"
                            SELECT 
	                            VRP.idVRP
	                            , PC.idPC, PC.descrPC, PC.modeloPC, PC.logradouroPC, PC.numeroPC, PC.bairroPC
                                , PC.cepPC, PC.latitudePC, PC.longitudePC, PC.imagemPC, PC.idCidade, CIDADE.descCidade
                                , PC.idNumCel, PC.tempoEnvioMinutos, PC.fatorMultVaz, PC.statusVazao, PC.statusPC
                            FROM vrp_horninksys.pontocritico AS PC
                            INNER JOIN vrp_horninksys.cidade AS CIDADE ON CIDADE.idCidade = PC.idCidade
                            INNER JOIN vrp_horninksys.vrppc AS VRPPC ON VRPPC.idPC = PC.idPC
                            INNER JOIN vrp_horninksys.vrp AS VRP ON VRP.idVRP = VRPPC.idVRP 
                        ";

            if (idVRP > 0)
            {
                query += " WHERE VRP.idVRP = @idVRP;";
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
                            var ret = new PontoCriticoModel()
                            {
                                idPC = int.Parse(reader["idPC"].ToString()),
                                descPC = reader["descPC"].ToString(),
                                modeloPC = reader["modeloPC"].ToString(),
                                logradouroPC = reader["logradouroPC"].ToString(),
                                numeroPC = reader["numeroPC"].ToString(),
                                bairroPC = reader["bairroPC"].ToString(),
                                cepPC = reader["cepPC"].ToString(),
                                latitudePC = decimal.Parse(reader["latitudePC"].ToString()),
                                longitudePC = decimal.Parse(reader["longitudePC"].ToString()),
                                imagemPC = reader["imagemPC"].ToString(),
                                idCidade = int.Parse(reader["idCidade"].ToString()),
                                descCidade = reader["descCidade"].ToString(),
                                idNumCel = int.Parse(reader["idNumCel"].ToString()),
                                tempoEnvioMinutos = int.Parse(reader["tempoEnvioMinutos"].ToString()),
                                fatorMultVaz = int.Parse(reader["fatorMultVaz"].ToString()),
                                statusVazao = reader["statusVazao"].ToString() == "1" ? true : false,
                                statusPC = reader["statusPC"].ToString() == "1" ? true : false
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

        public List<HistoricoPontoCriticoModel> ListaHistoricoPC(ParamListaHistoricoPCModel objParametros)
        {
            var query = "";
            MySqlDataReader reader = null;
            List<HistoricoPontoCriticoModel> listaRetorno = new List<HistoricoPontoCriticoModel>();

            if (objParametros.dataInicial != null && objParametros.dataFinal != null)
            {
                query = @"
                            SELECT 
	                            HistPC.idHisPC, HistPC.idPC, HistPC.pressaoPC, HistPC.vazaoPC
                                , HistPC.tensaoBateriaPC, HistPC.dataHoraPC 
                            FROM vrp_horninksys.historicopontocritico AS HistPC
                            WHERE HistPC.idPC = @idPC
                            AND DATE(HistPC.dataHoraPC) >= @dataInicial
                            AND DATE(HistPC.dataHoraPC) <= @dataFinal
                            ORDER BY HistPC.dataHoraPC DESC
                            LIMIT @linhas;
                        ";
            }
            else
            {
                query = @"
                            SELECT 
	                            HistPC.idHisPC, HistPC.idPC, HistPC.pressaoPC, HistPC.vazaoPC
                                , HistPC.tensaoBateriaPC, HistPC.dataHoraPC 
                            FROM vrp_horninksys.historicopontocritico AS HistPC
                            WHERE HistPC.idPC = @idPC
                            ORDER BY HistPC.dataHoraPC DESC
                            LIMIT @linhas;
                        ";
            }

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@idPC", MySqlDbType.Int32);
                com.Parameters["@idPC"].Value = objParametros.idPC;
                com.Parameters.Add("@dataInicial", MySqlDbType.Date);
                com.Parameters["@dataInicial"].Value = objParametros.dataInicial;
                com.Parameters.Add("@dataFinal", MySqlDbType.Date);
                com.Parameters["@dataFinal"].Value = objParametros.dataFinal;
                com.Parameters.Add("@linhas", MySqlDbType.Int32);
                com.Parameters["@linhas"].Value = objParametros.linhas;
                con.Open();
                try
                {
                    reader = com.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var ret = new HistoricoPontoCriticoModel()
                            {
                                idHisPC = int.Parse(reader["idHisPC"].ToString()),
                                idPC = int.Parse(reader["idPC"].ToString()),
                                pressaoPC = decimal.Parse(reader["pressaoPC"].ToString()),
                                vazaoPC = decimal.Parse(reader["vazaoPC"].ToString()),
                                tensaoBateriaPC = decimal.Parse(reader["tensaoBateriaPC"].ToString()),
                                dataHoraPC = DateTime.Parse(reader["dataHoraPC"].ToString())
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
