using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Trilhas.Data.Enums;
using Trilhas.Data.Model;
using Trilhas.Data.Model.Eventos;
using Trilhas.Models.Evento.Relatorios;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    [Authorize(Roles = "Administrador,Secretaria,Coordenador")]
    public class ListaInscricaoManualController : Controller
    {
        private readonly EventoService _eventoService;

        public ListaInscricaoManualController(EventoService eventoService)
        {
            _eventoService = eventoService;
        }

        public IActionResult ListaIndividual(long horarioId, long eventoId)
        {
            RelatorioListaPresencaViewModel vm = new RelatorioListaPresencaViewModel();
			vm.Relatorio = "RELATÓRIO DE CREDENCIAMENTO";
            Evento evento = _eventoService.RecuperarEventoListaPresenca(eventoId);
            EventoHorario eventoHorario = evento.Horarios.FirstOrDefault(x => x.Id == horarioId);
            //EventoHorario eventoHorario = _eventoService.RecuperarEventoHorario(horarioId);

            vm.Evento.EventoNome = evento.Curso.Descricao.ToString();
            vm.Evento.EventoSigla = evento.Curso.Sigla.ToString();
            vm.Evento.EntidadeNome = evento.EntidadeDemandante.Nome.ToString();

            if (evento.Curso.Modalidade == EnumModalidade.EAD)
            {
                vm.Evento.LocalId = 0;
                vm.Evento.LocalNome = "EaD";
            }
            else
            {
                vm.Evento.LocalId = evento.Local.Id;
                vm.Evento.LocalNome = evento.Local.Nome;
            }

            vm.Evento.DataInicio = evento.Agenda().DataHoraInicio;
            vm.Evento.DataFim = evento.Agenda().DataHoraFim;
            vm.DataInicio = eventoHorario.DataHoraInicio;
            vm.DataFim = eventoHorario.DataHoraFim;
            vm.DataAtual = DateTime.Now;

            if (evento.ListaDeInscricao != null)
            {
                var listaInscritos = evento.ListaDeInscricao.PessoasInscritas();

                foreach (var inscrito in listaInscritos)
                {
                    vm.ListaInscritos.Add(new InscritoListaPresencaManualViewModel()
                    {
                        NumeroFuncional = inscrito.NumeroFuncional,
                        CursistaNome = inscrito.NomeSocial ?? inscrito.Nome,
						CursistaCPF = FormatadorDeDados.FormatarCPF(inscrito.Cpf),
                        EntidadeNome = inscrito.Entidade.Nome,
                        EntidadeSigla = inscrito.Entidade.Sigla
                    });
                }

                vm.ListaInscritos = vm.ListaInscritos.OrderBy(x => x.CursistaNome).ToList();
            }

            return View(vm);
        }

        public IActionResult ListaCompleta(long eventoId)
        {
            RelatorioListaPresencaViewModel vm = new RelatorioListaPresencaViewModel();

            Evento evento = _eventoService.RecuperarEventoListaPresenca(eventoId);
            vm.Evento.EventoNome = evento.Curso.Descricao.ToString();
            vm.Evento.EventoSigla = evento.Curso.Sigla.ToString();
            vm.Evento.EntidadeNome = evento.EntidadeDemandante.Nome.ToString();
            vm.Evento.DataInicio = evento.Agenda().DataHoraInicio;
            vm.Evento.DataFim = evento.Agenda().DataHoraFim;

            if (evento.Curso.Modalidade == EnumModalidade.EAD)
            {
                vm.Evento.LocalId = 0;
                vm.Evento.LocalNome = "Ead";
            }
            else
            {
                vm.Evento.LocalId = evento.Local.Id;
                vm.Evento.LocalNome = evento.Local.Nome;
            }

            foreach (var horario in evento.Horarios)
            {
                vm.Datas.Add(new PeriodoListaPresencaViewModel()
                {
                    Data = horario.DataHoraInicio.Date,
                    HoraInicio = horario.DataHoraInicio,
                    HoraFim = horario.DataHoraFim
                });
            }

            vm.DataAtual = DateTime.Now;

            if (evento.ListaDeInscricao != null)
            {
                var listaInscritos = evento.ListaDeInscricao.PessoasInscritas();

                foreach (var inscrito in listaInscritos)
                {
                    vm.ListaInscritos.Add(new InscritoListaPresencaManualViewModel()
                    {
                        NumeroFuncional = inscrito.NumeroFuncional,
                        CursistaNome = inscrito.NomeSocial ?? inscrito.Nome,
						CursistaCPF = FormatadorDeDados.FormatarCPF(inscrito.Cpf),
                        EntidadeNome = inscrito.Entidade.Nome,
                        EntidadeSigla = inscrito.Entidade.Sigla
                    });
                }

                vm.ListaInscritos = vm.ListaInscritos.OrderBy(x => x.CursistaNome).ToList();
            }

            return View(vm);
        }
    }
}