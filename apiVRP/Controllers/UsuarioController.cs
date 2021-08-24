using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usuario.Data.Models;
using Usuario.Data.Repositories;

namespace apiVRP.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        GeradorSenhasController _geradorSenha = new GeradorSenhasController();
        private readonly UsuarioRepository _usuarioRepository;

        IConfiguration Configuration;
        private IHostingEnvironment _hostingEnvironment;
        public UsuarioController(IConfiguration iConfig, IHostingEnvironment hostingEnvironment)
        {
            Configuration = iConfig;
            _hostingEnvironment = hostingEnvironment;
            _usuarioRepository = new UsuarioRepository(Configuration);
        }

        [Route("{senhaURL?}")]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult GeraSenhaCripto(string senhaURL = "")
        {
            try
            {
                GeradorSenhasController objSenha = new GeradorSenhasController();
                object objRetornoSenha;
                if (senhaURL.Length < 8)
                {
                    string senha = objSenha.GerarSenha(8);
                    var senhaCrypto = objSenha.GerarHashString(senha);
                    objRetornoSenha = new { senha, senhaCrypto };
                }
                else
                {
                    var senhaCrypto = objSenha.GerarHashString(senhaURL);
                    objRetornoSenha = new { senhaCrypto };
                }

                return Ok(objRetornoSenha);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("{cpf}/{senha}")]
        [HttpGet]
        [EnableCors("_myAllowSpecificOrigins")]
        [Produces("application/json")]
        public IActionResult LoginUsuario(string cpf, string senha)
        {
            GeradorSenhasController geradorSenhasController = new GeradorSenhasController();
            var senhaCripto = geradorSenhasController.GerarHashString(senha);

            if (cpf != null && senha != null)
            {
                var ret = _usuarioRepository.LoginUsuario(cpf, senhaCripto);

                if (ret != null && ret.idUsuario > 0)
                {
                    if (ret.statusUsuario)
                    {
                        return Ok(ret);
                    }
                    else
                    {
                        ret = new UsuarioModel();
                        ret.erroMensagem = "Usuário inativo no sistema.";
                        return Ok(ret);
                    }
                }
                else
                {
                    ret.erroMensagem = "Usuário não encontrado.";
                    return Ok(ret);
                }
            }
            else
            {
                return BadRequest("Campos obrigatórios inválidos.");
            }
        }

        [Route("{cpf?}")]
        [HttpGet]
        [EnableCors("_myAllowSpecificOrigins")]
        [Produces("application/json")]
        public IActionResult BuscarUsuario(string cpf = "")
        {
            if (cpf != null)
            {
                var ret = _usuarioRepository.BuscarUsuario(cpf);

                return Ok(ret);
            }
            else
            {
                return BadRequest("Campos obrigatórios inválidos.");
            }
        }

        [Route("{idPerfil?}")]
        [HttpGet]
        [EnableCors("_myAllowSpecificOrigins")]
        [Produces("application/json")]
        public IActionResult BuscarPerfil(int idPerfil = 0)
        {
            var ret = _usuarioRepository.BuscarPerfil(idPerfil);

            return Ok(ret);
        }

        [HttpPost]
        [EnableCors("_myAllowSpecificOrigins")]
        [Produces("application/json")]
        public IActionResult ManterUsuario(UsuarioModel usuarioModel)
        {
            var ret = _usuarioRepository.ManterUsuario(usuarioModel);

            if (ret == "OK")
            {
                if (usuarioModel.senhaUsuario != null)
                {
                    usuarioModel.senhaUsuario = _geradorSenha.GerarHashString(usuarioModel.senhaUsuario);
                    ret = _usuarioRepository.AlteraSenhaUsuario(usuarioModel.idUsuario, usuarioModel.senhaUsuario);
                }
            }

            return Ok(ret);
        }

        [Route("{idUsuario}/{statusUsuario}")]
        [HttpGet]
        [EnableCors("_myAllowSpecificOrigins")]
        [Produces("application/json")]
        public IActionResult AlteraStatusUsuario(int idUsuario, bool statusUsuario)
        {
            var ret = _usuarioRepository.AlteraStatusUsuario(idUsuario, statusUsuario);

            return Ok(ret);
        }

        [HttpPost]
        [EnableCors("_myAllowSpecificOrigins")]
        [Produces("application/json")]
        public IActionResult InserirUsuario(UsuarioModel usuarioModel)
        {
            usuarioModel.senhaUsuario = _geradorSenha.GerarHashString(usuarioModel.senhaUsuario);
            var ret = _usuarioRepository.InserirUsuario(usuarioModel);

            return Ok(ret);
        }
    }
}
