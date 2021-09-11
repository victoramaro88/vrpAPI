﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        #region Construtor
        IConfiguration Configuration;
        private IHostingEnvironment _hostingEnvironment;
        public VRPController(IConfiguration iConfig, IHostingEnvironment hostingEnvironment)
        {
            Configuration = iConfig;
            _hostingEnvironment = hostingEnvironment;
            Configuration = iConfig;
            _appVRPRepo = new VRPRepository(Configuration);
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
            string resp = "";

            if (idVRP > 0)
            {
                try
                {
                    var retInfo = _appVRPRepo.PesquisaVRPParamAdc(idVRP);
                    HistoricoVRPModel objVRP = new HistoricoVRPModel();

                    //-> Verifica se está no horário de inserir o histórico e validar a pressão, senão aguarda.
                    if(retInfo.dataUltimoRegistro.AddMinutes(retInfo.tempoEnvioMinutos) < DateTime.Now)
                    {
                        objVRP.temperatura = temperatura;
                        objVRP.chuva = 0; //-> RECEBER ESTE PARÂMETRO DA BASE DO EDER.
                        objVRP.pressaoMont = pressaoMont;
                        objVRP.pressaoJus = pressaoJus;
                        // objVRP.vazao = Math.Round((vazao > 0 ? (((retInfo.fatorMultVaz*60) / vazao) * 60)/1000 : 0), 2);//-> m³/hora
                        objVRP.vazao = Math.Round((vazao > 0 ? ((retInfo.fatorMultVaz) / vazao) : 0), 2);//-> Litros/segundo
                        objVRP.volume = 0; //-> FAZER O CÁLCULO DO VOLUME AQUI
                        objVRP.tensaoBat = tensaoBat;
                        objVRP.idVRP = idVRP;
                        resp = _appVRPRepo.InsereHistVRP(objVRP);

                        if (resp == "OK")
                        {
                            var parametros = _appVRPRepo.ListaParametrosVRP(idVRP);
                            var retornoPressao = ConfiguraVRP(parametros, retInfo);
                            retornoPressao.vazao = objVRP.vazao.ToString();

                            return Ok(retornoPressao);
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
