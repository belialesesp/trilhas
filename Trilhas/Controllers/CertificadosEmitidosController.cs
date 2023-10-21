using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Model.Certificados;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    [Authorize]
    public class CertificadosEmitidosController : DefaultController
    {
        private readonly CertificadoEmitidoService _service;
        private readonly CertificadoEmitidoMapper _mapper;

        public CertificadosEmitidosController(UserManager<IdentityUser> userManager, CertificadoEmitidoService service) : base(userManager)
        {
            _service = service;
            _mapper = new CertificadoEmitidoMapper();
        }

        [HttpGet]
		[Authorize(Roles = "Administrador,Secretaria")]
		public IActionResult Buscar(string nome, bool excluidos, int start = -1, int count = -1)
        {
            List<CertificadoEmitido> certificados = _service.RecuperarCertificados(nome, excluidos);

            var vm = _mapper.MapearGridCertificadoEmitido(certificados);

            return Json(vm);
        }

        [HttpGet]
		[Authorize(Roles = "Administrador,Secretaria")]
		public IActionResult Quantidade(string nome, bool exibirExcluidos)
        {
            int quantidade = _service.QuantidadeDeCertificados(nome, exibirExcluidos);
            return Json(quantidade);
        }

        [HttpDelete]
		[Authorize(Roles = "Administrador,Secretaria")]
		public IActionResult Excluir(long id)
        {
            try
            {
                _service.ExcluirCertificado(RecuperarUsuarioId(), id);
            }
            catch (RecordNotFoundException rex)
            {
                return BadRequest(rex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return new EmptyResult();
        }
    }
}