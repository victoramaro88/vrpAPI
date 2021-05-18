using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    }
}
