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
                                VRP.idVRP, VRP.descrVRP, VRP.modelo, VRP.logradouro, 
                                VRP.numero, VRP.bairro, VRP.cep, VRP.latitude, VRP.longitude,
                                VRP.imagem, VRP.idCidade, CIDADE.descCidade, VRP.idNumCel, VRP.tempoEnvioMinutos, 
                                VRP.fatorMultVaz, VRP.status
                            FROM vrp_horninksys.vrp AS VRP
                            INNER JOIN vrp_horninksys.cidade AS CIDADE ON CIDADE.idCidade = VRP.idCidade
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
                                descCidade = reader["descCidade"].ToString(),
                                idNumCel = int.Parse(reader["idNumCel"].ToString()),
                                tempoEnvioMinutos = int.Parse(reader["tempoEnvioMinutos"].ToString()),
                                fatorMultVaz = int.Parse(reader["fatorMultVaz"].ToString()),
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
                            (temperatura, pressaoMont, pressaoJus, vazao, tensaoBat, dataHora, idVRP)
                            VALUES
                            (
                                @temperatura, @pressaoMont, @pressaoJus, 
                                @vazao, @tensaoBat, (SELECT NOW()), @idVRP
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
                com.Parameters.Add("@tensaoBat", MySqlDbType.Decimal);
                com.Parameters["@tensaoBat"].Value = objVRP.tensaoBat;
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

        public string AlteraItemParaMetroVRP(ParametrosVRPModel objParametro)
        {
            string retorno = "";
            var query = "";

            query = @"
                        UPDATE `vrp_horninksys`.`parametrosvrp`
                        SET
                        `pressao` = @pressao,
                        `horaInicial` = @horaInicial,
                        `horaFinal` = @horaFinal,
                        `flStatus` = @flStatus
                        WHERE `idParametro` = @idParametro;

                        SELECT 'OK' AS Retorno;
                     ";

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
                    {
                        MySqlDataReader reader = null;
                        MySqlCommand com = new MySqlCommand(query, con);
                        com.Parameters.Add("@pressao", MySqlDbType.Decimal);
                        com.Parameters["@pressao"].Value = objParametro.pressao;
                        com.Parameters.Add("@horaInicial", MySqlDbType.VarChar);
                        com.Parameters["@horaInicial"].Value = objParametro.horaInicial;
                        com.Parameters.Add("@horaFinal", MySqlDbType.VarChar);
                        com.Parameters["@horaFinal"].Value = objParametro.horaFinal;
                        com.Parameters.Add("@flStatus", MySqlDbType.Bit);
                        com.Parameters["@flStatus"].Value = objParametro.flStatus;
                        com.Parameters.Add("@idParametro", MySqlDbType.Int32);
                        com.Parameters["@idParametro"].Value = objParametro.idParametro;
                        con.Open();
                        try
                        {
                            reader = com.ExecuteReader();
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    retorno = reader["Retorno"].ToString();
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
                    }

            return retorno;
        }

        public string ManterParametrosVRP(List<ParametrosVRPModel> listaParametros)
        {
            string retorno = "";
            var query = "";

            #region REMOVENDO TODOS PARÂMETROS
            query = @"
                        DELETE FROM `vrp_horninksys`.`parametrosvrp`
                        WHERE idVRP = @idVRP;

                        SELECT 'OK' AS Retorno;
                     ";

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlDataReader reader = null;
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@idVRP", MySqlDbType.Int32);
                com.Parameters["@idVRP"].Value = listaParametros[0].idVRP;
                con.Open();
                try
                {
                    reader = com.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            retorno = reader["Retorno"].ToString();
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
            }
            #endregion

            #region INSERINDO OS NOVOS PARÂMETROS
            if (retorno == "OK")
            {
                foreach (var item in listaParametros)
                {
                    query = @"INSERT INTO `vrp_horninksys`.`parametrosvrp`
                            (`pressao`,`horaInicial`,`horaFinal`,`idVRP`,`flStatus`)
                            VALUES
                            (@pressao, @horaInicial, @horaFinal, @idVRP, 1); 

                        SELECT 'OK' AS Retorno;";

                    using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
                    {
                        MySqlDataReader reader = null;
                        MySqlCommand com = new MySqlCommand(query, con);
                        com.Parameters.Add("@pressao", MySqlDbType.Decimal);
                        com.Parameters["@pressao"].Value = item.pressao;
                        com.Parameters.Add("@horaInicial", MySqlDbType.VarChar);
                        com.Parameters["@horaInicial"].Value = item.horaInicial;
                        com.Parameters.Add("@horaFinal", MySqlDbType.VarChar);
                        com.Parameters["@horaFinal"].Value = item.horaFinal;
                        com.Parameters.Add("@idVRP", MySqlDbType.Int32);
                        com.Parameters["@idVRP"].Value = item.idVRP;
                        con.Open();
                        try
                        {
                            reader = com.ExecuteReader();
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    retorno = reader["Retorno"].ToString();
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
                    }
                }
            }
            #endregion

            return retorno;
        }

        public ParamatrosAdcVRPModel ListaParametroAdicionalVRP(int idVRP)
        {
            MySqlDataReader reader = null;
            ParamatrosAdcVRPModel objRetorno = new ParamatrosAdcVRPModel();

            var query = @"
                            SELECT
	                            idVRP, pressao, flStatus
                            FROM parametrosadcvrp
                            WHERE idVRP = @idVRP AND flStatus = 1;
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
                        reader.Read();
                        objRetorno.idVRP = int.Parse(reader["idVRP"].ToString());
                        objRetorno.pressao = decimal.Parse(reader["pressao"].ToString());
                        objRetorno.flStatus = reader["flStatus"].ToString() == "1" ? true : false;
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

                return objRetorno;
            }
        }

        public ParamVRPADCModel PesquisaVRPParamAdc(int idVRP)
        {
            MySqlDataReader reader = null;
            ParamVRPADCModel objRetorno = new ParamVRPADCModel();

            var query = @"
                            SELECT 
		                        V.idVRP, V.TempoEnvioMinutos, V.fatorMultVaz,
		                        P.pressao, P.flStatus, 
		                        (SELECT dataHora
			                        FROM historicovrp
			                        ORDER BY dataHora DESC LIMIT 1) AS dataUltimoRegistro
	                        FROM vrp V
	                        LEFT JOIN parametrosadcvrp P ON P.idVRP = V.idVRP
	                        WHERE V.idVRP = @idVRP;
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
                        reader.Read();
                        objRetorno.idVRP = int.Parse(reader["idVRP"].ToString());
                        objRetorno.tempoEnvioMinutos = int.Parse(reader["TempoEnvioMinutos"].ToString());
                        objRetorno.fatorMultVaz = int.Parse(reader["fatorMultVaz"].ToString());
                        objRetorno.pressao = decimal.Parse(reader["pressao"].ToString());
                        objRetorno.flStatusADC = reader["flStatus"].ToString() == "1" ? true : false;
                        objRetorno.dataUltimoRegistro = reader["dataUltimoRegistro"] != DBNull.Value
                                                    ? DateTime.Parse(reader["dataUltimoRegistro"].ToString()) : default(DateTime);
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

                return objRetorno;
            }
        }

        public List<HistoricoVRPModel> ListaHistoricoVRP(ParamListaHistoricoModel objParametros)
        {
            var query = "";
            MySqlDataReader reader = null;
            List<HistoricoVRPModel> listaRetorno = new List<HistoricoVRPModel>();

            if(objParametros.dataInicial != null && objParametros.dataFinal != null)
            {
                query = @"
                            SELECT 
		                            idHistorico, temperatura, pressaoMont, pressaoJus, tensaoBat, vazao, dataHora, idVRP
                            FROM vrp_horninksys.historicovrp 
                            WHERE idVRP = @idVRP
                            AND DATE(dataHora) >= @dataInicial
                            AND DATE(dataHora) <= @dataFinal
                            ORDER BY dataHora DESC
                            LIMIT @linhas;
                        ";
            }
            else
            {
                query = @"
                            SELECT 
		                            idHistorico, temperatura, pressaoMont, pressaoJus, vazao, tensaoBat, dataHora, idVRP
                            FROM vrp_horninksys.historicovrp 
                            WHERE idVRP = @idVRP
                            ORDER BY dataHora DESC
                            LIMIT @linhas;
                        ";
            }

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@idVRP", MySqlDbType.Int32);
                com.Parameters["@idVRP"].Value = objParametros.idVRP;
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
                            var ret = new HistoricoVRPModel()
                            {
                                idHistorico = int.Parse(reader["idHistorico"].ToString()),
                                temperatura = decimal.Parse(reader["temperatura"].ToString()),
                                pressaoMont = decimal.Parse(reader["pressaoMont"].ToString()),
                                pressaoJus = decimal.Parse(reader["pressaoJus"].ToString()),
                                vazao = decimal.Parse(reader["vazao"].ToString()),
                                tensaoBat = decimal.Parse(reader["tensaoBat"].ToString()),
                                dataHora = DateTime.Parse(reader["dataHora"].ToString()),
                                idVRP = int.Parse(reader["idVRP"].ToString())
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
