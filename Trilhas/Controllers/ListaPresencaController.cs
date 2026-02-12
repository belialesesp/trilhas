using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Controllers.Mappers;
using Trilhas.Data.Model.Eventos;
using Trilhas.Models.Evento.ListaPresenca;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    [Authorize(Roles = "Administrador,GESE,Gestor,GEDTH")]
    public class ListaPresencaController : DefaultController
    {
        private readonly EventoService _eventoService;
        private readonly ListaPresencaMapper _mapper;
        private readonly PessoaService _pessoaService;
        private readonly ListaPresencaService _listaPresencaService;

        public ListaPresencaController(UserManager<IdentityUser> userManager,
            EventoService eventoService,
            PessoaService pessoaService,
            ListaPresencaService listaPresencaService) : base(userManager)
        {
            _eventoService = eventoService;
            _mapper = new ListaPresencaMapper();
            _pessoaService = pessoaService;
            _listaPresencaService = listaPresencaService;
        }

        [HttpPost]
        public IActionResult SalvarListaPresenca([FromBody] ListaPresencaInscritosViewModel vm)
        {
            try
            {
                var eventoHorario = _eventoService.RecuperarEventoHorario(vm.EventoHorarioId);
                var pessoa = _pessoaService.RecuperarPessoa(vm.PessoaId);

                _listaPresencaService.SalvarListaPresenca(new RegistroDePresenca
                {
                    EventoHorario = eventoHorario,
                    Id = vm.Id,
                    Presente = vm.Presente,
                    Pessoa = pessoa
                });

                if (eventoHorario.Evento.Finalizado)
                {
                    var evento = _eventoService.RecuperarEventoCompleto(eventoHorario.Evento.Id);
                    _eventoService.AlterarFrequencia(evento, vm.PessoaId);
                }

                return JsonFormResponse(vm.Id);
            }
            catch (Exception ex)
            {
                return JsonFormResponse(ex);
            }
        }

        [HttpGet]
        public IActionResult BuscarDadosParaRegistrarPresenca(string codigoBarras, long id)
        {
            try
            {
                Evento evento;

                if (!string.IsNullOrEmpty(codigoBarras))
                {
                    evento = ValidarCodigoBarras(codigoBarras);
                }
                else
                {
                    evento = _eventoService.RecuperarEventoListaPresenca(id);
                }

                if (evento.ListaDeInscricao != null)
                {
                    evento.ListaDeInscricao.Inscritos = evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue).ToList();
                }
                else
                {
                    evento.ListaDeInscricao = new ListaDeInscricao();
                }

                var vm = new ListaPresencaViewModel
                {
                    EventoHorarios = _mapper.MappearListaPresencaEventoHorario(evento.Horarios),
                    EventoTitulo = evento.Curso.Sigla + " - " + evento.Curso.Titulo,
                    EventoId = evento.Id
                };

                if (vm.EventoHorarios.Any(x => x.Selecionar))
                {
                    vm.Inscritos = AtualizarListaInscritos(vm.EventoHorarios.FirstOrDefault(x => x.Selecionar).EventoHorarioId, evento.ListaDeInscricao.Inscritos);
                }
                else
                {
                    vm.Inscritos = _mapper.MappearListaPresencaInscritos(evento.ListaDeInscricao.Inscritos);
                }

                //Registrar Presença para consultas realizadas pelo codigo de barras
                if (vm.EventoHorarios.Any(x => x.Selecionar) && !string.IsNullOrEmpty(codigoBarras) && vm.Inscritos.Any(x => !x.Presente))
                {
                    var horarioSelecionado = vm.EventoHorarios.FirstOrDefault(x => x.Selecionar);

                    _listaPresencaService.SalvarListaPresenca(new RegistroDePresenca
                    {
                        EventoHorario = evento.Horarios.FirstOrDefault(x => x.Id == horarioSelecionado.EventoHorarioId),
                        Id = vm.Inscritos[0].Id,
                        Presente = true,
                        Pessoa = evento.ListaDeInscricao.Inscritos[0].Cursista
                    });

                    vm.Inscritos[0].Presente = true;
                }

                return Json(vm);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        public IActionResult AtualizarListaInscritorPorHorario(long id, long eventoHorarioId, string codigoBarras)
        {
            try
            {
                Evento evento;

                if (!string.IsNullOrEmpty(codigoBarras))
                {
                    evento = ValidarCodigoBarras(codigoBarras);
                }
                else
                {
                    evento = _eventoService.RecuperarEventoListaPresenca(id);
                }

                if (evento.ListaDeInscricao != null)
                {
                    evento.ListaDeInscricao.Inscritos = evento.ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue).ToList();
                }
                else
                {
                    evento.ListaDeInscricao = new ListaDeInscricao();
                }

                var vm = AtualizarListaInscritos(eventoHorarioId, evento.ListaDeInscricao.Inscritos);

                return Json(vm);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        private List<ListaPresencaInscritosViewModel> AtualizarListaInscritos(long eventoHorarioId, List<Inscrito> inscritos)
        {
            var listaInscritos = new List<ListaPresencaInscritosViewModel>();

            foreach (var inscrito in inscritos)
            {
                var listaPresenca = _listaPresencaService.RecuperarListaPresenca(eventoHorarioId, inscrito.Cursista.Id);

                listaInscritos.Add(new ListaPresencaInscritosViewModel
                {
                    EventoHorarioId = eventoHorarioId,
                    PessoaId = inscrito.Cursista.Id,
                    PessoaNome = inscrito.Cursista.NomeSocial ?? inscrito.Cursista.Nome,
                    Presente = listaPresenca == null ? false : listaPresenca.Presente,
                    Id = listaPresenca == null ? 0 : listaPresenca.Id
                });
            }

            return listaInscritos;
        }

        private Evento ValidarCodigoBarras(string codigoBarras)
        {
            if (string.IsNullOrEmpty(codigoBarras) || !codigoBarras.Contains("-"))
            {
                throw new Exception("Código de Barras Incorreto.");
            }

            var codigo = codigoBarras.Split("-");
            var evento = _eventoService.RecuperarEventoListaPresenca(Convert.ToInt64(codigo[1]));

            if (evento == null)
            {
                throw new Exception("Código de Barras Incorreto.");
            }

            evento.ListaDeInscricao.Inscritos = evento.ListaDeInscricao.Inscritos.Where(x => x.Cursista.Id == Convert.ToInt64(codigo[0])).ToList();

            if (!evento.ListaDeInscricao.Inscritos.Any())
            {
                throw new Exception("Código de Barras Incorreto.");
            }

            return evento;
        }
    }
}