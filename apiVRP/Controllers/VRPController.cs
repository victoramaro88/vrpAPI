using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        [Route("{idVRP}/{temperatura}/{pressaoMont}/{pressaoJus}/{vazao}")]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult InsereHistVRP(int idVRP, decimal temperatura, decimal pressaoMont, decimal pressaoJus, decimal vazao)
        {
            //string resp = "";
            string resp = "OK";

            if (idVRP > 0)
            {
                try
                {
                    //passar valores para obj para inserir
                    //resp = _appVRPRepo.InsereHistVRP(objVRP);

                    if (resp == "OK")
                    {
                        var parametros = _appVRPRepo.ListaParametrosVRP(idVRP);
                        var retornoPressao = ConfiguraVRP(parametros);

                        return Ok(retornoPressao);
                    }
                    else
                    {
                        return BadRequest("Erro: " + resp);
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

        [NonAction]
        private decimal ConfiguraVRP(List<ParametrosVRPModel> listaParametros)
        {
            decimal resp = 0;
            try
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
                            resp = item.pressao;
                        }
                    }
                    else
                    {
                        var horaL = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59) - horaLiga;
                        horaL = horaL.Add(TimeSpan.FromSeconds(1));
                        if (horaLiga <= horaAgora && horaAgora <= horaLiga.Add(horaL).AddHours(horaDesLiga.Hour))
                        {
                            resp = item.pressao;
                        }
                    }
                }

                return resp;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //-> VERIFICAÇÃO DOS HORÁRIOS PARA SETAR A PRESSÃO.
        //[NonAction]
        //private string ConfiguraPortas(List<ControlePlacaModel> listaPinos, decimal temperatura, decimal temperaturaAlerta, decimal umidade)
        //{
        //    try
        //    {
        //        #region Verificando horário para ligar/desligar as portas
        //        listaPinos.Where(l => l.ctrPlcPno == "D6").FirstOrDefault().ctrPlcStt = false;
        //        listaPinos.Where(l => l.ctrPlcPno == "D7").FirstOrDefault().ctrPlcStt = false;
        //        listaPinos.Where(l => l.ctrPlcPno == "D8").FirstOrDefault().ctrPlcStt = false;
        //        // -> Controle dos ares condicionados
        //        foreach (var item in listaPinos)
        //        {
        //            switch (item.ctrPlcPno)
        //            {
        //                case "D6":
        //                    // -> Ar 1
        //                    if (item.listaHorarios.Count > 0)
        //                    {
        //                        foreach (var itemHora in item.listaHorarios)
        //                        {
        //                            DateTime horaAgora = DateTime.Now;
        //                            DateTime horaLiga = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Int32.Parse(itemHora.ctrHfnHorLig.Substring(0, 2)), Int32.Parse(itemHora.ctrHfnHorLig.Substring(3)), 0);
        //                            DateTime horaDesLiga = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Int32.Parse(itemHora.ctrHfnHorDlg.Substring(0, 2)), Int32.Parse(itemHora.ctrHfnHorDlg.Substring(3)), 0);

        //                            if (horaLiga < horaDesLiga)
        //                            {
        //                                if (horaLiga <= horaAgora && horaAgora <= horaDesLiga)
        //                                {
        //                                    listaPinos.Where(l => l.ctrPlcPno == "D6").FirstOrDefault().ctrPlcStt = true;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                var horaL = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59) - horaLiga;
        //                                horaL = horaL.Add(TimeSpan.FromSeconds(1));
        //                                if (horaLiga <= horaAgora && horaAgora <= horaLiga.Add(horaL).AddHours(horaDesLiga.Hour))
        //                                {
        //                                    listaPinos.Where(l => l.ctrPlcPno == "D6").FirstOrDefault().ctrPlcStt = true;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    break;
        //                case "D7":
        //                    // -> Ar 2
        //                    if (item.listaHorarios.Count > 0)
        //                    {
        //                        foreach (var itemHora in item.listaHorarios)
        //                        {
        //                            DateTime horaAgora = DateTime.Now;
        //                            DateTime horaLiga = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Int32.Parse(itemHora.ctrHfnHorLig.Substring(0, 2)), Int32.Parse(itemHora.ctrHfnHorLig.Substring(3)), 0);
        //                            DateTime horaDesLiga = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Int32.Parse(itemHora.ctrHfnHorDlg.Substring(0, 2)), Int32.Parse(itemHora.ctrHfnHorDlg.Substring(3)), 0);

        //                            if (horaLiga < horaDesLiga)
        //                            {
        //                                if (horaLiga <= horaAgora && horaAgora <= horaDesLiga)
        //                                {
        //                                    listaPinos.Where(l => l.ctrPlcPno == "D7").FirstOrDefault().ctrPlcStt = true;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                var horaL = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59) - horaLiga;
        //                                horaL = horaL.Add(TimeSpan.FromSeconds(1));
        //                                if (horaLiga <= horaAgora && horaAgora <= horaLiga.Add(horaL).AddHours(horaDesLiga.Hour))
        //                                {
        //                                    listaPinos.Where(l => l.ctrPlcPno == "D7").FirstOrDefault().ctrPlcStt = true;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    break;
        //                case "D8":
        //                    // -> Ar 3
        //                    if (item.listaHorarios.Count > 0)
        //                    {
        //                        foreach (var itemHora in item.listaHorarios)
        //                        {
        //                            DateTime horaAgora = DateTime.Now;
        //                            DateTime horaLiga = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Int32.Parse(itemHora.ctrHfnHorLig.Substring(0, 2)), Int32.Parse(itemHora.ctrHfnHorLig.Substring(3)), 0);
        //                            DateTime horaDesLiga = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Int32.Parse(itemHora.ctrHfnHorDlg.Substring(0, 2)), Int32.Parse(itemHora.ctrHfnHorDlg.Substring(3)), 0);

        //                            if (horaLiga < horaDesLiga)
        //                            {
        //                                if (horaLiga <= horaAgora && horaAgora <= horaDesLiga)
        //                                {
        //                                    listaPinos.Where(l => l.ctrPlcPno == "D8").FirstOrDefault().ctrPlcStt = true;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                var horaL = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59) - horaLiga;
        //                                horaL = horaL.Add(TimeSpan.FromSeconds(1));
        //                                if (horaLiga <= horaAgora && horaAgora <= horaLiga.Add(horaL).AddHours(horaDesLiga.Hour))
        //                                {
        //                                    listaPinos.Where(l => l.ctrPlcPno == "D8").FirstOrDefault().ctrPlcStt = true;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    break;

        //                default:
        //                    break;
        //            }
        //        }

        //        // -> Se a temperatura estiver mais alta que a permitida, liga todos os ares, e envia mensagem para os usuários a cada 1 hora.
        //        if (temperatura > temperaturaAlerta)
        //        {
        //            listaPinos.Where(l => l.ctrPlcPno == "D6").FirstOrDefault().ctrPlcStt = true;
        //            listaPinos.Where(l => l.ctrPlcPno == "D7").FirstOrDefault().ctrPlcStt = true;
        //            listaPinos.Where(l => l.ctrPlcPno == "D8").FirstOrDefault().ctrPlcStt = true;

        //            EnviaNotificacao(temperatura, umidade);
        //        }
        //        #endregion

        //        // -> Atualizando as portas dos ares no BD.
        //        var retSucssManterAres = _controleDataCenterRepo.ManterConfigAr(listaPinos);

        //        return retSucssManterAres;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

    }
}
