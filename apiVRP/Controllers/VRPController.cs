using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PC.Data.Repositories;
using System;
using System.Collections.Generic;
using VRP.Data.Models;
using VRP.Data.Repositories;

namespace apiVRP.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VRPController : ControllerBase
    {
        private readonly VRPRepository _appVRPRepo;
        private readonly PontoCriticoRepository _appPCRepo;

        #region Construtor
        IConfiguration Configuration;
        private IHostingEnvironment _hostingEnvironment;
        public VRPController(IConfiguration iConfig, IHostingEnvironment hostingEnvironment)
        {
            Configuration = iConfig;
            _hostingEnvironment = hostingEnvironment;
            Configuration = iConfig;
            _appVRPRepo = new VRPRepository(Configuration);
            _appPCRepo = new PontoCriticoRepository(Configuration);
        }
        #endregion

        [Route("{idVRP?}")]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult ListaVRP(int idVRP = 0)
        {
            List<VRPModel> listaRetorno = new List<VRPModel>();

            try
            {
                listaRetorno = _appVRPRepo.ListaVRP(idVRP);

                if (listaRetorno.Count > 0)
                {
                    return Ok(listaRetorno);
                }
                else
                {
                    return Ok("Sem informações de retorno.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Erro: " + ex.Message);
            }
        }

        [Route("{idVRP?}")]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult ListaParametrosVRP(int idVRP = 0)
        {
            List<ParametrosVRPModel> listaRetorno = new List<ParametrosVRPModel>();

            try
            {
                listaRetorno = _appVRPRepo.ListaParametrosVRP(idVRP);

                if (listaRetorno.Count > 0)
                {
                    return Ok(listaRetorno);
                }
                else
                {
                    return Ok("Sem informações de retorno.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Erro: " + ex.Message);
            }
        }

        [Route("{idCidade?}")]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult ListaCidade(int idCidade = 0)
        {
            List<CidadeModel> listaRetorno = new List<CidadeModel>();

            try
            {
                listaRetorno = _appVRPRepo.ListaCidade(idCidade);

                if (listaRetorno.Count > 0)
                {
                    return Ok(listaRetorno);
                }
                else
                {
                    return Ok("Sem informações de retorno.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Erro: " + ex.Message);
            }
        }

        [Route("{idVRP}/{temperatura}/{pressaoMont}/{pressaoJus}/{vazao}/{tensaoBat}")]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult InsereHistVRP(int idVRP, decimal temperatura, decimal pressaoMont, decimal pressaoJus, decimal vazao, decimal tensaoBat)
        {
            List<string> resp = new List<string>();

            if (idVRP > 0)
            {
                try
                {
                    var vrp = _appVRPRepo.ListaVRP(idVRP);

                    var retInfo = _appVRPRepo.PesquisaVRPParamAdc(idVRP);
                    HistoricoVRPModel objVRP = new HistoricoVRPModel();

                    //-> Verifica se está no horário de inserir o histórico e validar a pressão, senão aguarda.
                    if(retInfo.dataUltimoRegistro.AddMinutes(retInfo.tempoEnvioMinutos) < DateTime.Now)
                    {
                        // ->Recebendo a previsão do tempo da carga da IA (Eder).
                        var previsao = _appVRPRepo.PrevisaoDoTempo();
                        //objVRP.temperatura = temperatura;
                        objVRP.temperatura = previsao.temperatura;
                        objVRP.chuva = previsao.chuva;
                        objVRP.pressaoMont = pressaoMont;
                        objVRP.pressaoJus = pressaoJus;
                        objVRP.vazao = Math.Round((vazao > 0 ? ((retInfo.fatorMultVaz) / vazao) : 0), 2);//-> Litros/segundo
                        objVRP.volume = 0;
                        objVRP.tensaoBat = tensaoBat;
                        objVRP.idVRP = idVRP;
                        resp = _appVRPRepo.InsereHistVRP(objVRP); //-> Insere Histórico, calcula o volume e retorna-o para repassar para a IA.

                        if (resp[0] == "OK")
                        {
                            //-> Adicionando os valores para enviar para a IA.
                            ParametrosIAModel objIA = new ParametrosIAModel();
                            objIA.volume_1 = decimal.Parse(resp[1]);
                            objIA.pressão_jusante = objVRP.pressaoJus;
                            objIA.vazão_instântanea_1 = objVRP.vazao;
                            objIA.PC = _appPCRepo.ListaUltimaPressaoPC(objVRP.idVRP).pressaoPC;
                            objIA.hora = DateTime.Now.ToShortTimeString();
                            objIA.dia_semana = RetornaDiaDaSemana();
                            string _abertura = _appVRPRepo.RetornaHoraAberturaVRP(objVRP.idVRP).horaInicial;
                            objIA.abertura = _abertura != null ? _abertura : "00:00";
                            string _fechamento = _appVRPRepo.RetornaHoraFechamentoVRP(objVRP.idVRP).horaFinal;
                            objIA.fechamento = _fechamento != null ? _fechamento : "00:00";
                            
                            ParametroRetornoModel objretornoPressao = new ParametroRetornoModel();

                            if (vrp[0].tipParamCod == 1)
                            {
                                var parametros = _appVRPRepo.ListaParametrosVRP(idVRP);
                                objretornoPressao = ConfiguraVRP(parametros, retInfo);
                                objretornoPressao.vazao = objVRP.vazao.ToString();
                            }
                            else if (vrp[0].tipParamCod == 2)
                            {
                                //-> AQUI SERÁ CHAMADO A API DA INTELIGÊNCIA ARTIFICIAL. ****************************************************
                                objretornoPressao.idVRP = objVRP.idVRP;
                                objretornoPressao.pressao = 0; // ->  A PRESSÃO DA IA ENTRARÁ AQUI!!!
                                objretornoPressao.vazao = objVRP.vazao.ToString();
                                objretornoPressao.msg = "OK";
                            }
                            else if (vrp[0].tipParamCod == 3)
                            {
                                objretornoPressao.idVRP = objVRP.idVRP;
                                objretornoPressao.pressao = 0;
                                objretornoPressao.vazao = objVRP.vazao.ToString();
                                objretornoPressao.msg = "DESATIVADA";
                            }

                            return Ok(objretornoPressao);
                        }
                        else
                        {
                            return BadRequest("Erro: " + resp);
                        }
                    }
                    else
                    {
                        ParametroRetornoModel objRet = new ParametroRetornoModel();
                        objRet.msg = "Aguardando horário.";
                        objRet.vazao = Math.Round((vazao > 0 ? ((retInfo.fatorMultVaz) / vazao) : 0), 2).ToString();//-> Litros/segundo;
                        return Ok(objRet);
                    }                    
                }
                catch (Exception ex)
                {
                    return BadRequest("Erro: " + ex.Message);
                }
            }
            else
            {
                return BadRequest("Parâmetros inválidos.");
            }
        }

        [Produces("application/json")]
        [HttpPost]
        public IActionResult AlteraItemParaMetroVRP([FromBody] ParametrosVRPModel objParametro)
        {
            try
            {
                if (objParametro != null && objParametro.idParametro > 0)
                {
                    var resposta = _appVRPRepo.AlteraItemParaMetroVRP(objParametro);

                    return Ok(resposta);
                }
                else
                {
                    return Ok("Sem informações de retorno.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Erro: " + ex.Message);
            }
        }

        [Produces("application/json")]
        [HttpPost]
        public IActionResult ManterParametrosVRP([FromBody] List<ParametrosVRPModel> listaParametros)
        {
            try
            {
                if (listaParametros != null && listaParametros.Count > 0)
                {
                    var resposta = _appVRPRepo.ManterParametrosVRP(listaParametros);

                    return Ok(resposta);
                }
                else
                {
                    return Ok("Sem informações de retorno.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Erro: " + ex.Message);
            }
        }

        [Produces("application/json")]
        [HttpPost]
        public IActionResult ListaHistoricoVRP([FromBody] ParamListaHistoricoModel objParametros)
        {
            List<HistoricoVRPModel> listaRetorno = new List<HistoricoVRPModel>();

            if (objParametros.idVRP > 0)
            {
                if (objParametros.linhas <= 0)
                {
                    objParametros.linhas = 100;
                }

                try
                {
                    listaRetorno = _appVRPRepo.ListaHistoricoVRP(objParametros);

                    if (listaRetorno.Count > 0)
                    {
                        return Ok(listaRetorno);
                    }
                    else
                    {
                        return Ok("Sem informações de retorno.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Erro: " + ex.Message);
                }
            }
            else
            {
                return BadRequest("Id da VRP Inválido!");
            }
        }

        //[Produces("application/json")]
        //[HttpPost]
        //public IActionResult InsereHistVRPPOST(ParametrosEntradaModel objEntrada)
        //{
        //    //string resp = "";
        //    string resp = "OK";

        //    if (objEntrada.idVRP > 0)
        //    {
        //        try
        //        {
        //            //passar valores para obj para inserir
        //            //resp = _appVRPRepo.InsereHistVRP(objVRP);

        //            if (resp == "OK")
        //            {
        //                var parametros = _appVRPRepo.ListaParametrosVRP(objEntrada.idVRP);
        //                var retornoPressao = ConfiguraVRP(parametros);

        //                return Ok(retornoPressao);
        //            }
        //            else
        //            {
        //                return BadRequest("Erro: " + resp);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest("Erro: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("Parâmetros inválidos.");
        //    }
        //}

        [NonAction]
        private ParametroRetornoModel ConfiguraVRP(List<ParametrosVRPModel> listaParametros, ParamVRPADCModel retInfo)
        {
            ParametroRetornoModel objRet = new ParametroRetornoModel();
            try
            {                
                if(retInfo.flStatusADC)
                {
                    objRet.idVRP = retInfo.idVRP;
                    objRet.pressao = retInfo.pressao;
                    objRet.msg = "OK";
                }
                else
                {
                    foreach (var item in listaParametros)
                    {
                        DateTime horaAgora = DateTime.Now;
                        DateTime horaLiga = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Int32.Parse(item.horaInicial.Substring(0, 2)), Int32.Parse(item.horaInicial.Substring(3)), 0);
                        DateTime horaDesLiga = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Int32.Parse(item.horaFinal.Substring(0, 2)), Int32.Parse(item.horaFinal.Substring(3)), 0);

                        //-> Pegando o dia da semana para validar a pressão correta
                        String diaSemana = DateTime.Now.DayOfWeek.ToString();

                        if (horaLiga < horaDesLiga)
                        {
                            if (horaLiga <= horaAgora && horaAgora <= horaDesLiga)
                            {
                                objRet.idVRP = listaParametros[0].idVRP;
                                if(diaSemana == "Saturday" || diaSemana == "Sunday")
                                {
                                    objRet.pressao = item.pressaoFds;
                                }
                                else
                                {
                                    objRet.pressao = item.pressao;
                                }
                                objRet.msg = "OK";
                            }
                        }
                        else
                        {
                            var horaL = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59) - horaLiga;
                            horaL = horaL.Add(TimeSpan.FromSeconds(1));
                            if (horaLiga <= horaAgora && horaAgora <= horaLiga.Add(horaL).AddHours(horaDesLiga.Hour))
                            {
                                objRet.idVRP = listaParametros[0].idVRP;
                                if(diaSemana == "Saturday" || diaSemana == "Sunday")
                                {
                                    objRet.pressao = item.pressaoFds;
                                }
                                else
                                {
                                    objRet.pressao = item.pressao;
                                }
                                objRet.msg = "OK";
                            }
                        }
                    }
                }                

                return objRet;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [Route("{idNumCel?}")]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult ListaNumeroCelularOperadora(int idNumCel = 0)
        {
            List<NumeroCelOperModel> listaRetorno = new List<NumeroCelOperModel>();

            try
            {
                listaRetorno = _appVRPRepo.ListaNumeroCelularOperadora(idNumCel);

                if (listaRetorno.Count > 0)
                {
                    return Ok(listaRetorno);
                }
                else
                {
                    return Ok("Sem informações de retorno.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Erro: " + ex.Message);
            }
        }
    
        [Produces("application/json")]
        [HttpPost]
        public IActionResult ManterVRP([FromBody] VRPModel objVRP)
        {
            try
            {
                if (objVRP != null)
                {
                    var resposta = _appVRPRepo.ManterVRP(objVRP);

                    return Ok(resposta);
                }
                else
                {
                    return Ok("Sem informações de retorno.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Erro: " + ex.Message);
            }
        }

        [Produces("application/json")]
        [HttpGet]
        public IActionResult PrevisaoDoTempo()
        {
            PrevisaoDoTempoModel objRetorno = new PrevisaoDoTempoModel();

            try
            {
                objRetorno = _appVRPRepo.PrevisaoDoTempo();

                if (objRetorno != null)
                {
                    return Ok(objRetorno);
                }
                else
                {
                    return Ok("Sem informações de retorno.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Erro: " + ex.Message);
            }
        }

        [NonAction]
        private int RetornaDiaDaSemana()
        {
            String diaSemana = DateTime.Now.DayOfWeek.ToString();

            switch (diaSemana)
            {
                case "Monday":
                    return 0;
                case "Tuesday":
                    return 1;
                case "Wednesday":
                    return 2;
                case "Thursday":
                    return 3;
                case "Friday":
                    return 4;
                case "Saturday":
                    return 5;
                case "Sunday":
                    return 6;
                default:
                    return 9;
            }
        }



        [Route("{idNumCel}")]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult VerificaNumCelVRP(int idNumCel)
        {
            List<VRPModel> listaRetorno = new List<VRPModel>();

            try
            {
                listaRetorno = _appVRPRepo.ListaVRP(idNumCel);

                if (listaRetorno.Count > 0)
                {
                    return Ok(listaRetorno);
                }
                else
                {
                    return Ok("Sem informações de retorno.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Erro: " + ex.Message);
            }
        }

        [Produces("application/json")]
        [HttpGet]
        public IActionResult ListaIDsNumCelUsados()
        {
            List<int> listaRetorno = new List<int>();

            try
            {
                listaRetorno = _appVRPRepo.ListaIDsNumCelUsados();

                if (listaRetorno.Count > 0)
                {
                    return Ok(listaRetorno);
                }
                else
                {
                    return Ok("Sem informações de retorno.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Erro: " + ex.Message);
            }
        }
    }
}
