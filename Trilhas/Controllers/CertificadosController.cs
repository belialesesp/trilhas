using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Enums;
using Trilhas.Data.Model.Certificados;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Models.Certificado;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    [Authorize]
    public class CertificadosController : DefaultController
    {
        private readonly CertificadoService _service;
        private readonly CertificadoEmitidoService _certificadoEmitidoService;
        private readonly EventoService _eventoService;
        private readonly CertificadoMapper _mapper;

        public CertificadosController(
            UserManager<IdentityUser> userManager, 
            CertificadoService service, 
            EventoService eventoService, 
            CertificadoEmitidoService certificadoEmitidoService)
            : base(userManager)
        {
            _service = service;
            _eventoService = eventoService;
            _certificadoEmitidoService = certificadoEmitidoService;
            _mapper = new CertificadoMapper();
        }

        [Authorize(Roles = "Administrador,GESE")]
        public IActionResult Index(long inscricaoId, long docenteId, long eventoId)
        {
            EmissaoCertificadoViewModel vm;

            if (inscricaoId > 0)
            {
                var inscricao = _eventoService.RecuperarInscricao(inscricaoId);

                if (inscricao == null)
                {
                    return this.BadRequest();
                }

                Certificado certificado = null;

                if (inscricao.Situacao == EnumSituacaoCursista.CERTIFICADO)
                {
                    if (inscricao.ListaDeInscricao.Evento.Certificado != null)
                    {
                        certificado = inscricao.ListaDeInscricao.Evento.Certificado;
                    }
                }
                else if (inscricao.Situacao == EnumSituacaoCursista.DECLARADO)
                {
                    certificado = inscricao.ListaDeInscricao.Evento.DeclaracaoCursista;
                }

                if (certificado == null)
                {
                    return new EmptyResult();
                }

                var condigoAutenticacao = _service.CodigoAutenticacaoEletronica();

                vm = _mapper.MapearEmissaoCertificado(inscricao, condigoAutenticacao, certificado.Dados);

                _certificadoEmitidoService.GerarCertificadoEmitido(RecuperarUsuarioId(), condigoAutenticacao, vm.Dados, inscricao.Cursista);
            }
            else
            {
                var evento = _eventoService.RecuperarEventoCompleto(eventoId);
                var docente = evento.Horarios.Select(x => x.Docente).FirstOrDefault(x => x.Id == docenteId);
                Certificado declaracaoDocente = evento.DeclaracaoDocente;

                if (declaracaoDocente == null)
                {
                    return new EmptyResult();
                }

                vm = _mapper.MapearEmissaoDeclaracaoDocente(docente, evento, declaracaoDocente.Dados);
            }

            return View(vm);
        }

        [Authorize(Roles = "Administrador,GESE")]
        public IActionResult Preview(long id)
        {
            Certificado certificado = _service.RecuperarCertificado(id, null);

            EmissaoCertificadoViewModel vm = _mapper.MaperPreviewCertificado(certificado);

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,GESE")]
        public IActionResult Salvar([FromBody] SalvarCertificadoViewModel vm)
        {
            try
            {
                ValidarCadastroCertificado(vm);

                Certificado certificado;

                if (vm.Id > 0)
                {
                    certificado = AtualizarCertificado(vm);
                }
                else
                {
                    certificado = CriarCertificado(vm);
                }

                AtualizarCertificadoPadrao(vm);

                _service.SalvarCertificado(RecuperarUsuarioId(), certificado);

                return JsonFormResponse(vm);
            }
            catch (TrilhasException tex)
            {
                return JsonErrorFormResponse(tex);
            }
            catch (Exception ex)
            {
                return JsonErrorFormResponse(ex, "Ocorreu um erro ao salvar o registro.");
            }
        }

        private void AtualizarCertificadoPadrao(SalvarCertificadoViewModel vm)
        {
            var certificadoPadrao = _service.RecuperarCertificadoPadrao(vm.TipoCertificado);

            if (certificadoPadrao == null || certificadoPadrao.Id == vm.Id)
            {
                vm.Padrao = true;
                return;
            }

            if (vm.Padrao && certificadoPadrao.Id != vm.Id)
            {
                certificadoPadrao.Padrao = false;
            }
        }

        public Certificado AtualizarCertificado(SalvarCertificadoViewModel vm)
        {
            Certificado certificado = _service.RecuperarCertificado(vm.Id, null);
            certificado.Nome = vm.Nome;
            certificado.Dados = vm.Dados;
            certificado.Padrao = vm.Padrao;
            certificado.TipoCertificado = vm.TipoCertificado;

            return certificado;
        }

        public Certificado CriarCertificado(SalvarCertificadoViewModel vm)
        {
            Certificado certificado = new Certificado
            {
                Nome = vm.Nome,
                Dados = vm.Dados,
                Padrao = vm.Padrao,
                TipoCertificado = vm.TipoCertificado
            };

            return certificado;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,GESE")]
        public IActionResult Buscar(string nome, bool excluidos, EnumTipoCertificado? tipoCertificado, int start = -1, int count = -1)
        {
            List<Certificado> certificados = _service.RecuperarCertificados(nome, excluidos, tipoCertificado, start, count);

            var vm = _mapper.MapearGridCertificado(certificados);

            return Json(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,GESE")]
        public IActionResult Quantidade(string nome, bool exibirExcluidos, EnumTipoCertificado? tipoCertificado, int start = -1, int count = -1)
        {
            int quantidade = _service.QuantidadeDeCertificados(nome, exibirExcluidos, tipoCertificado);
            return Json(quantidade);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,GESE")]
        public IActionResult Recuperar(long id, EnumTipoCertificado? tipoCertificado)
        {
            try
            {
                Certificado certificado = _service.RecuperarCertificado(id, tipoCertificado);

                if (certificado == null)
                    throw new Exception("Certificado não encontrado");

                var vm = _mapper.MapearCertificado(certificado);

                return Json(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrador,GESE")]
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

        private void ValidarCadastroCertificado(SalvarCertificadoViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Nome))
            {
                ModelState.AddModelError("Nome", "Preencha o nome do Certificado.");
            }
            if (string.IsNullOrWhiteSpace(vm.Dados))
            {
                ModelState.AddModelError("Dados", "Preencha o conteúdo do Certificado.");
            }

            if (!ModelState.IsValid)
            {
                throw new Exception("Preencha o formulário corretamente.");
            }
        }

        public IActionResult Validar()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Consultar([FromBody] ConsultaCertificadoViewModel request)
        {
            bool valido = false;
            string mensagem = "Não há certificado válido para este código de autenticação.";

            var certificado = _certificadoEmitidoService.RecuperarCertificado(request.CodigoAutenticidade);

            if (certificado != null)
            {
                valido = true;
                mensagem = "Certificado válido - Autenticidade verificada com sucesso!";
            }

            return Json(new { Valido = valido, Mensagem = mensagem });
        }
    }
}