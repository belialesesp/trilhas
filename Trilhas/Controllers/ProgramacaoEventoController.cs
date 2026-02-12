using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Trilhas.Data.Enums;
using Trilhas.Data.Model.Eventos;
using Trilhas.Models.Evento.Relatorios;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    [Authorize(Roles = "Administrador,GESE,Gestor,GEDTH")]
    public class ProgramacaoEventoController : Controller
    {
        private readonly EventoService _eventoService;

        public ProgramacaoEventoController(EventoService eventoService)
        {
            _eventoService = eventoService;
        }

        public IActionResult Index(int mes, int ano)
        {
            DateTime dataInicio = new DateTime(ano, mes, 1);
            DateTime dataFim = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));

            RelatorioProgramacaoEventosViewModel vm = new RelatorioProgramacaoEventosViewModel();
            List<Evento> listaEvento = _eventoService.PesquisarProgramacaoEventos(dataInicio, dataFim);

            foreach (var evento in listaEvento)
            {
                var horario = evento.Horarios.First();

                if (evento.Curso.Modalidade == EnumModalidade.EAD)
                {
                    vm.ListaEaD.Add(new ProgramacaoEventoCursoViewModel
                    {
                        Eixo = evento.Curso.Estacao.Eixo.Descricao,
                        Sigla = evento.Curso.Sigla,
                        Curso = evento.Curso.Titulo,
                        PublicoAlvo = evento.Curso.PublicoAlvo,
                        CargaHoraria = evento.Curso.CargaHorariaTotal().ToString() + " h",
                        PeriodoInscricao = evento.Agenda().DataHoraInscricaoInicio.ToString("dd/MM/yyyy") + "-" + evento.Agenda().DataHoraInscricaoFim.ToString("dd/MM/yyyy"),
                        NumeroVagas = evento.Agenda().NumeroVagas,
                        PeriodoRealizacao = evento.Agenda().DataHoraInicio.ToString("dd/MM/yyyy") + "-" + evento.Agenda().DataHoraFim.ToString("dd/MM/yyyy"),
                        Horario = evento.Horarios.Last().DataHoraInicio.ToString("HH:mm") + " às " + evento.Horarios.Last().DataHoraFim.ToString("HH:mm"),
                        Local = "Site EAD",
                        Docente = horario.Docente.Pessoa.NomeSocial ?? horario.Docente.Pessoa.Nome,
                        GEDTH = evento.GEDTH.NomeSocial ?? evento.GEDTH.Nome
                    });
				} else if(evento.Curso.Modalidade == EnumModalidade.PRESENCIAL)
				{
					vm.ListaPresencial.Add(new ProgramacaoEventoCursoViewModel {
						Eixo = evento.Curso.Estacao.Eixo.Descricao,
						Sigla = evento.Curso.Sigla,
						Curso = evento.Curso.Titulo,
						PublicoAlvo = evento.Curso.PublicoAlvo,
						CargaHoraria = evento.Curso.CargaHorariaTotal().ToString() + " h",
						PeriodoInscricao = evento.Agenda().DataHoraInscricaoInicio.ToString("dd/MM/yyyy") + "-" + evento.Agenda().DataHoraInscricaoFim.ToString("dd/MM/yyyy"),
						NumeroVagas = evento.Agenda().NumeroVagas,
						PeriodoRealizacao = evento.Agenda().DataHoraInicio.ToString("dd/MM/yyyy") + "-" + evento.Agenda().DataHoraFim.ToString("dd/MM/yyyy"),
						Horario = evento.Horarios.Last().DataHoraInicio.ToString("HH:mm") + " às " + evento.Horarios.Last().DataHoraFim.ToString("HH:mm"),
						Local = evento.Local.Nome,
						Docente = horario.Docente.Pessoa.NomeSocial ?? horario.Docente.Pessoa.Nome,
						GEDTH = evento.GEDTH.NomeSocial ?? evento.GEDTH.Nome
					});
				}
                else
                {
                    vm.ListaSemiPresencial.Add(new ProgramacaoEventoCursoViewModel
                    {
                        Eixo = evento.Curso.Estacao.Eixo.Descricao,
                        Sigla = evento.Curso.Sigla,
                        Curso = evento.Curso.Titulo,
                        PublicoAlvo = evento.Curso.PublicoAlvo,
                        CargaHoraria = evento.Curso.CargaHorariaTotal().ToString() + " h",
                        PeriodoInscricao = evento.Agenda().DataHoraInscricaoInicio.ToString("dd/MM/yyyy") + "-" + evento.Agenda().DataHoraInscricaoFim.ToString("dd/MM/yyyy"),
                        NumeroVagas = evento.Agenda().NumeroVagas,
                        PeriodoRealizacao = evento.Agenda().DataHoraInicio.ToString("dd/MM/yyyy") + "-" + evento.Agenda().DataHoraFim.ToString("dd/MM/yyyy"),
                        Horario = evento.Horarios.Last().DataHoraInicio.ToString("HH:mm") + " às " + evento.Horarios.Last().DataHoraFim.ToString("HH:mm"),
                        Local = evento.Local.Nome,
                        Docente = horario.Docente.Pessoa.NomeSocial ?? horario.Docente.Pessoa.Nome,
                        GEDTH = evento.GEDTH.NomeSocial ?? evento.GEDTH.Nome
                    });
                }
            }

            vm.Mes = new CultureInfo("pt-BR").DateTimeFormat.GetMonthName(dataInicio.Month) + "/" + dataInicio.Year.ToString();
            vm.TotalVagas = listaEvento.Sum(x => x.QuantidadeDeVagas());

            return View(vm);
        }

        public IActionResult Individual(long id)
        {
            RelatorioProgramacaoEventoViewModel vm = new RelatorioProgramacaoEventoViewModel();
            Evento evento = _eventoService.RecuperarEventoEdicao(id);
            vm.Nome = evento.Curso.Titulo;
            vm.Sigla = evento.Curso.Sigla;
            vm.Turma = "";
            vm.Modalidade = evento.Curso.Modalidade.ToString();
            vm.CargaHoraria = evento.Curso.CargaHorariaTotal().ToString() + " h";

            vm.PeriodoInscricao = evento.Agenda().DataHoraInscricaoInicio.Day + " de " + (new CultureInfo("pt-BR").DateTimeFormat.GetMonthName(evento.Agenda().DataHoraInscricaoInicio.Month)).ToUpper() + " de " + evento.Agenda().DataHoraInscricaoInicio.Year + " às " + evento.Agenda().DataHoraInscricaoFim.Day + " de " + (new CultureInfo("pt-BR").DateTimeFormat.GetMonthName(evento.Agenda().DataHoraInscricaoFim.Month)).ToUpper() + " de " + evento.Agenda().DataHoraInscricaoFim.Year;

            vm.PeriodoRealizacao = evento.Agenda().DataHoraInicio.Day + " de " + (new CultureInfo("pt-BR").DateTimeFormat.GetMonthName(evento.Agenda().DataHoraInicio.Month)).ToUpper() + " de " + evento.Agenda().DataHoraInicio.Year + " às " + evento.Agenda().DataHoraFim.Day + " de " + (new CultureInfo("pt-BR").DateTimeFormat.GetMonthName(evento.Agenda().DataHoraFim.Month)).ToUpper() + " de " + evento.Agenda().DataHoraFim.Year;

            if (evento.Curso.Modalidade != EnumModalidade.EAD)
            {
                vm.Local = evento.Local.Nome;
            }

            var horario = evento.Horarios.First();
            vm.Docente = horario.Docente.Pessoa.NomeSocial ?? horario.Docente.Pessoa.Nome;
            vm.NumeroVagas = evento.QuantidadeDeVagas();
            vm.PublicoAlvo = evento.Curso.PublicoAlvo;

            return View(vm);
        }
    }
}