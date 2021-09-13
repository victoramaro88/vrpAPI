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
                                descPC = reader["descrPC"].ToString(),
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

        public PCParamHistModel ListaPontoCriticoParametrosHist(int idPC)
        {
            MySqlDataReader reader = null;
            PCParamHistModel objRetorno = new PCParamHistModel();

            var query = @"
                            SELECT 
                                PC.idPC, PC.tempoEnvioMinutos, PC.fatorMultVaz, PC.statusVazao
                                , (SELECT dataHoraPC
		                            FROM vrp_horninksys.historicopontocritico
		                            ORDER BY dataHoraPC DESC LIMIT 1) AS dataUltimoRegistro
                            FROM vrp_horninksys.pontocritico AS PC
                            WHERE PC.idPC = @idPC;
                        ";

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
                        reader.Read();
                        objRetorno.idPC = int.Parse(reader["idPC"].ToString());
                        objRetorno.tempoEnvioMinutos = int.Parse(reader["TempoEnvioMinutos"].ToString());
                        objRetorno.fatorMultVaz = int.Parse(reader["fatorMultVaz"].ToString());
                        objRetorno.statusVazao = reader["statusVazao"].ToString() == "1" ? true : false;
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

        public string InserirHistoricoPC(HistoricoPontoCriticoModel objHistPC)
        {
            MySqlDataReader reader = null;
            string resp = "";

            var query = @"
                            INSERT INTO `vrp_horninksys`.`historicopontocritico`
                            (`idPC`, `pressaoPC`,`vazaoPC`,`tensaoBateriaPC`, `dataHoraPC`)
                            VALUES
                            (
                                @idPC,
                                @pressaoPC,
                                @vazaoPC,
                                @tensaoBateriaPC,
                                (SELECT NOW())
                            );
                            SELECT 'OK' AS Retorno;
                        ";

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@idPC", MySqlDbType.Int32);
                com.Parameters["@idPC"].Value = objHistPC.idPC;
                com.Parameters.Add("@pressaoPC", MySqlDbType.Decimal);
                com.Parameters["@pressaoPC"].Value = objHistPC.pressaoPC;
                com.Parameters.Add("@vazaoPC", MySqlDbType.Decimal);
                com.Parameters["@vazaoPC"].Value = objHistPC.vazaoPC;
                com.Parameters.Add("@tensaoBateriaPC", MySqlDbType.Decimal);
                com.Parameters["@tensaoBateriaPC"].Value = objHistPC.tensaoBateriaPC;
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

        public string ManterPC(PontoCriticoModel objPC)
        {
            string retorno = "";
            var query = "";

            if (objPC.idPC > 0)
            {
                query = @"
                            UPDATE `vrp_horninksys`.`pontocritico`
                            SET
                            `idPC` = @idPC,
                            `descrPC` = @descPC,
                            `modeloPC` = @modeloPC,
                            `logradouroPC` = @logradouroPC,
                            `numeroPC` = @numeroPC,
                            `bairroPC` = @bairroPC,
                            `cepPC` = @cepPC,
                            `latitudePC` = @latitudePC,
                            `longitudePC` = @longitudePC,
                            `imagemPC` = @imagemPC,
                            `idCidade` = @idCidade,
                            `idNumCel` = @idNumCel,
                            `tempoEnvioMinutos` = @tempoEnvioMinutos,
                            `fatorMultVaz` = @fatorMultVaz,
                            `statusVazao` = @statusVazao,
                            `statusPC` = @statusPC
                            WHERE `idPC` = @idPC;

                            SELECT 'OK' AS Retorno;
                        ";

                using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
                {
                    MySqlDataReader reader = null;
                    MySqlCommand com = new MySqlCommand(query, con);
                    com.Parameters.Add("@idPC", MySqlDbType.Int32);
                    com.Parameters["@idPC"].Value = objPC.idPC;
                    com.Parameters.Add("@descPC", MySqlDbType.VarChar);
                    com.Parameters["@descPC"].Value = objPC.descPC;
                    com.Parameters.Add("@modeloPC", MySqlDbType.VarChar);
                    com.Parameters["@modeloPC"].Value = objPC.modeloPC;
                    com.Parameters.Add("@logradouroPC", MySqlDbType.VarChar);
                    com.Parameters["@logradouroPC"].Value = objPC.logradouroPC;
                    com.Parameters.Add("@numeroPC", MySqlDbType.VarChar);
                    com.Parameters["@numeroPC"].Value = objPC.numeroPC;
                    com.Parameters.Add("@bairroPC", MySqlDbType.VarChar);
                    com.Parameters["@bairroPC"].Value = objPC.bairroPC;
                    com.Parameters.Add("@cepPC", MySqlDbType.VarChar);
                    com.Parameters["@cepPC"].Value = objPC.cepPC;
                    com.Parameters.Add("@latitudePC", MySqlDbType.Decimal);
                    com.Parameters["@latitudePC"].Value = objPC.latitudePC;
                    com.Parameters.Add("@longitudePC", MySqlDbType.Decimal);
                    com.Parameters["@longitudePC"].Value = objPC.longitudePC;
                    com.Parameters.Add("@imagemPC", MySqlDbType.VarChar);
                    com.Parameters["@imagemPC"].Value = objPC.imagemPC;
                    com.Parameters.Add("@idCidade", MySqlDbType.Int32);
                    com.Parameters["@idCidade"].Value = objPC.idCidade;
                    com.Parameters.Add("@idNumCel", MySqlDbType.Int32);
                    com.Parameters["@idNumCel"].Value = objPC.idNumCel;
                    com.Parameters.Add("@tempoEnvioMinutos", MySqlDbType.Int32);
                    com.Parameters["@tempoEnvioMinutos"].Value = objPC.tempoEnvioMinutos;
                    com.Parameters.Add("@fatorMultVaz", MySqlDbType.Int32);
                    com.Parameters["@fatorMultVaz"].Value = objPC.fatorMultVaz;
                    com.Parameters.Add("@statusVazao", MySqlDbType.Bit);
                    com.Parameters["@statusVazao"].Value = objPC.statusVazao ? 1 : 0;
                    com.Parameters.Add("@statusPC", MySqlDbType.Bit);
                    com.Parameters["@statusPC"].Value = objPC.statusPC ? 1 : 0;
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
            else
            {
                query = @"
                            INSERT INTO `vrp_horninksys`.`pontocritico`
                            (`descrPC`, `modeloPC`, `logradouroPC`, `numeroPC`, `bairroPC`, `cepPC`,
                            `latitudePC`, `longitudePC`, `imagemPC`, `idCidade`, `idNumCel`, `tempoEnvioMinutos`,
                            `fatorMultVaz`, `statusVazao`, `statusPC`)
                            VALUES
                            (
                                @descPC,
                                @modeloPC,
                                @logradouroPC,
                                @numeroPC,
                                @bairroPC,
                                @cepPC,
                                @latitudePC,
                                @longitudePC,
                                @imagemPC,
                                @idCidade,
                                @idNumCel,
                                @tempoEnvioMinutos,
                                @fatorMultVaz,
                                @statusVazao,
                                @statusPC
                            );

                            SELECT 'OK' AS Retorno;
                        ";

                using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
                {
                    MySqlDataReader reader = null;
                    MySqlCommand com = new MySqlCommand(query, con);
                    com.Parameters.Add("@descPC", MySqlDbType.VarChar);
                    com.Parameters["@descPC"].Value = objPC.descPC;
                    com.Parameters.Add("@modeloPC", MySqlDbType.VarChar);
                    com.Parameters["@modeloPC"].Value = objPC.modeloPC;
                    com.Parameters.Add("@logradouroPC", MySqlDbType.VarChar);
                    com.Parameters["@logradouroPC"].Value = objPC.logradouroPC;
                    com.Parameters.Add("@numeroPC", MySqlDbType.VarChar);
                    com.Parameters["@numeroPC"].Value = objPC.numeroPC;
                    com.Parameters.Add("@bairroPC", MySqlDbType.VarChar);
                    com.Parameters["@bairroPC"].Value = objPC.bairroPC;
                    com.Parameters.Add("@cepPC", MySqlDbType.VarChar);
                    com.Parameters["@cepPC"].Value = objPC.cepPC;
                    com.Parameters.Add("@latitudePC", MySqlDbType.Decimal);
                    com.Parameters["@latitudePC"].Value = objPC.latitudePC;
                    com.Parameters.Add("@longitudePC", MySqlDbType.Decimal);
                    com.Parameters["@longitudePC"].Value = objPC.longitudePC;
                    com.Parameters.Add("@imagemPC", MySqlDbType.VarChar);
                    com.Parameters["@imagemPC"].Value = objPC.imagemPC;
                    com.Parameters.Add("@idCidade", MySqlDbType.Int32);
                    com.Parameters["@idCidade"].Value = objPC.idCidade;
                    com.Parameters.Add("@idNumCel", MySqlDbType.Int32);
                    com.Parameters["@idNumCel"].Value = objPC.idNumCel;
                    com.Parameters.Add("@tempoEnvioMinutos", MySqlDbType.Int32);
                    com.Parameters["@tempoEnvioMinutos"].Value = objPC.tempoEnvioMinutos;
                    com.Parameters.Add("@fatorMultVaz", MySqlDbType.Int32);
                    com.Parameters["@fatorMultVaz"].Value = objPC.fatorMultVaz;
                    com.Parameters.Add("@statusVazao", MySqlDbType.Bit);
                    com.Parameters["@statusVazao"].Value = objPC.statusVazao ? 1 : 0;
                    com.Parameters.Add("@statusPC", MySqlDbType.Bit);
                    com.Parameters["@statusPC"].Value = objPC.statusPC ? 1 : 0;
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

            return retorno;
        }

        public string VinculaVRP(int idVRP, int idPC)
        {
            MySqlDataReader reader = null;
            string resp = "";

            var query = @"

                            DELETE FROM `vrp_horninksys`.`vrppc`
                            WHERE idPC = @idPC;

                            INSERT INTO `vrp_horninksys`.`vrppc`
                            (`idVRP`, `idPC`)
                            VALUES
                            (
                                @idVRP,
                                @idPC
                            );

                            SELECT 'OK' AS Retorno;
                        ";

            using (MySqlConnection con = new MySqlConnection(_scDB_VRP))
            {
                MySqlCommand com = new MySqlCommand(query, con);
                com.Parameters.Add("@idPC", MySqlDbType.Int32);
                com.Parameters["@idPC"].Value = idPC;
                com.Parameters.Add("@idVRP", MySqlDbType.Int32);
                com.Parameters["@idVRP"].Value = idVRP;
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

        public VinculoVRPPCModel ListaVinculoVRP(int idPC)
        {
            MySqlDataReader reader = null;
            VinculoVRPPCModel objRetorno = new VinculoVRPPCModel();

            var query = @"
                            SELECT 
	                            idVRP, idPC
                            FROM vrp_horninksys.vrppc
                            WHERE idPC = @idPC;
                        ";

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
                        reader.Read();
                        objRetorno.idPC = int.Parse(reader["idPC"].ToString());
                        objRetorno.idVRP = int.Parse(reader["idVRP"].ToString());
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
    }
}
