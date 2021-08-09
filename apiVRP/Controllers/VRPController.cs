using Microsoft.AspNetCore.Hosting;
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

                    //-> Verifica se está no horário de inserir o histórico e validar a pressão, senão aguarda.
                    if(retInfo.dataUltimoRegistro.AddMinutes(retInfo.tempoEnvioMinutos) < DateTime.Now)
                    {
                        HistoricoVRPModel objVRP = new HistoricoVRPModel();
                        objVRP.temperatura = temperatura;
                        objVRP.pressaoMont = pressaoMont;
                        objVRP.pressaoJus = pressaoJus;
                        objVRP.vazao = vazao;
                        objVRP.tensaoBat = tensaoBat;
                        objVRP.idVRP = idVRP;
                        resp = _appVRPRepo.InsereHistVRP(objVRP);

                        if (resp == "OK")
                        {
                            var parametros = _appVRPRepo.ListaParametrosVRP(idVRP);
                            var retornoPressao = ConfiguraVRP(parametros, retInfo);

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

                        if (horaLiga < horaDesLiga)
                        {
                            if (horaLiga <= horaAgora && horaAgora <= horaDesLiga)
                            {
                                objRet.idVRP = listaParametros[0].idVRP;
                                objRet.pressao = item.pressao;
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
                                objRet.pressao = item.pressao;
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

        
    }
}
