using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PC.Data.Models;
using PC.Data.Repositories;
using System;
using System.Collections.Generic;
using VRP.Data.Models;
using VRP.Data.Repositories;

namespace apiVRP.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PontoCriticoController : ControllerBase
    {
        private readonly PontoCriticoRepository _appPCRepo;

        #region Construtor
        IConfiguration Configuration;
        private IHostingEnvironment _hostingEnvironment;
        public PontoCriticoController(IConfiguration iConfig, IHostingEnvironment hostingEnvironment)
        {
            Configuration = iConfig;
            _hostingEnvironment = hostingEnvironment;
            Configuration = iConfig;
            _appPCRepo = new PontoCriticoRepository(Configuration);
        }
        #endregion

        [Route("{idPC?}")]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult ListaPC(int idPC = 0)
        {
            List<PontoCriticoModel> listaRetorno = new List<PontoCriticoModel>();

            try
            {
                listaRetorno = _appPCRepo.ListaPontoCritico(idPC);

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
        public IActionResult ListaPontoCriticoByIdVRP(int idVRP = 0)
        {
            List<PontoCriticoModel> listaRetorno = new List<PontoCriticoModel>();

            try
            {
                listaRetorno = _appPCRepo.ListaPontoCriticoByIdVRP(idVRP);

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
        public IActionResult ListaHistoricoPC([FromBody] ParamListaHistoricoPCModel objParametros)
        {
            List<HistoricoPontoCriticoModel> listaRetorno = new List<HistoricoPontoCriticoModel>();

            if (objParametros.idPC > 0)
            {
                if (objParametros.linhas <= 0)
                {
                    objParametros.linhas = 100;
                }

                try
                {
                    listaRetorno = _appPCRepo.ListaHistoricoPC(objParametros);

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
                return BadRequest("Id do ponto crítico inválido!");
            }
        }

        [Route("{idPC}/{pressaoPC}/{vazaoPC}/{tensaoBateriaPC}")]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult InserirHistoricoPC(int idPC, decimal pressaoPC, decimal vazaoPC, decimal tensaoBateriaPC)
        {
            string resp = "";

            if (idPC > 0)
            {
                try
                {
                    var retInfo = _appPCRepo.ListaPontoCriticoParametrosHist(idPC);
                    HistoricoPontoCriticoModel objPC = new HistoricoPontoCriticoModel();

                    //-> Verifica se está no horário de inserir o histórico e validar a pressão, senão aguarda.
                    if (retInfo.dataUltimoRegistro.AddMinutes(retInfo.tempoEnvioMinutos) < DateTime.Now)
                    {
                        objPC.pressaoPC = pressaoPC;
                        objPC.vazaoPC =  Math.Round((vazaoPC > 0 ? ((retInfo.fatorMultVaz) / vazaoPC) : 0), 2);//-> Litros/segundo
                        objPC.tensaoBateriaPC = tensaoBateriaPC;
                        objPC.idPC = idPC;
                        resp = _appPCRepo.InserirHistoricoPC(objPC);

                        ParametroRetornoPCModel objRet = new ParametroRetornoPCModel();
                        objRet.idPC = idPC;
                        objRet.msg = resp;
                        objRet.vazao = objPC.vazaoPC.ToString();

                        return Ok(objRet);
                    }
                    else
                    {
                        ParametroRetornoPCModel objRet = new ParametroRetornoPCModel();
                        objRet.idPC = idPC;
                        objRet.vazao = Math.Round((vazaoPC > 0 ? ((retInfo.fatorMultVaz) / vazaoPC) : 0), 2).ToString();//-> Litros/segundo
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
        public IActionResult ManterPC([FromBody] PontoCriticoModel objPC)
        {
            try
            {
                if (objPC != null)
                {
                    var resposta = _appPCRepo.ManterPC(objPC);

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

        [Route("{idVRP}/{idPC}")]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult VinculaVRP(int idVRP, int idPC)
        {
            try
            {
                if (idPC > 0 && idVRP > 0)
                {
                    var resposta = _appPCRepo.VinculaVRP(idVRP, idPC);

                    return Ok(resposta);
                }
                else
                {
                    return Ok("Parâmetros inválidos.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Erro: " + ex.Message);
            }
        }

        [Route("{idPC}")]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult ListaVinculoVRP(int idPC)
        {
            try
            {
                if (idPC > 0)
                {
                    var resposta = _appPCRepo.ListaVinculoVRP(idPC);

                    if (resposta.idVRP > 0 && resposta.idPC > 0)
                    {
                        return Ok(resposta);
                    }
                    else
                    {
                        return Ok("Sem informações de retorno.");
                    }
                }
                else
                {
                    return Ok("Parâmetros inválidos.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Erro: " + ex.Message);
            }
        }
    }
}