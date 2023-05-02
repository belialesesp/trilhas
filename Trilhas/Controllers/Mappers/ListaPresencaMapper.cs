using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Data.Model.Eventos;
using Trilhas.Models.Evento.ListaPresenca;

namespace Trilhas.Controllers.Mappers
{
    public class ListaPresencaMapper
    {
        public List<ListaPresencaEventoHorariosViewModel> MappearListaPresencaEventoHorario(List<EventoHorario> horarios)
        {
            var lista = new List<ListaPresencaEventoHorariosViewModel>();

            foreach (var horario in horarios)
            {
                lista.Add(new ListaPresencaEventoHorariosViewModel
                {
                    DataHoraFim = horario.DataHoraFim,
                    DataHoraInicio = horario.DataHoraInicio,
                    EventoHorarioId = horario.Id,
                    Modulo = horario.Modulo.Nome,
                    Selecionar = false
                });
            }

            //1º Caso: Identifica se existe um horario dentro do periodo atual
            if (lista.Any(x => x.DataHoraInicio <= DateTime.Now && x.DataHoraFim > DateTime.Now))
            {
                var horaSelecionada = lista.FirstOrDefault(x => x.DataHoraInicio <= DateTime.Now && x.DataHoraFim > DateTime.Now);
                var idx = lista.LastIndexOf(horaSelecionada);
                lista[idx].Selecionar = true;
            }
            //2º Caso: Identificar se existe um horário superior a horario atual no mesmo dia
            else if (lista.Any(x => x.DataHoraInicio.Year == DateTime.Now.Year && x.DataHoraInicio.Month == DateTime.Now.Month && x.DataHoraInicio.Day == DateTime.Now.Day && x.DataHoraInicio.Hour >= DateTime.Now.Hour))
            {
                var horaSelecionada = lista.FirstOrDefault(x => x.DataHoraInicio.Year == DateTime.Now.Year && x.DataHoraInicio.Month == DateTime.Now.Month && x.DataHoraInicio.Day == DateTime.Now.Day && x.DataHoraInicio.Hour >= DateTime.Now.Hour);

                var idx = lista.LastIndexOf(horaSelecionada);
                lista[idx].Selecionar = true;
            }
            //3º Caso: Identificar se existe um horário no dia atual
            else if (lista.Any(x => x.DataHoraInicio.Year == DateTime.Now.Year && x.DataHoraInicio.Month == DateTime.Now.Month && x.DataHoraInicio.Day == DateTime.Now.Day))
            {
                var horaSelecionada = lista.FirstOrDefault(x => x.DataHoraInicio.Year == DateTime.Now.Year && x.DataHoraInicio.Month == DateTime.Now.Month && x.DataHoraInicio.Day == DateTime.Now.Day);

                var idx = lista.LastIndexOf(horaSelecionada);
                lista[idx].Selecionar = true;
            }

            return lista.OrderBy(x => x.DataHoraInicio).ToList();
        }

        public List<ListaPresencaInscritosViewModel> MappearListaPresencaInscritos(List<Inscrito> inscritos)
        {
            var lista = new List<ListaPresencaInscritosViewModel>();

            foreach (var inscrito in inscritos)
            {
                lista.Add(new ListaPresencaInscritosViewModel
                {
                    PessoaId = inscrito.Cursista.Id,
                    PessoaNome = inscrito.Cursista.NomeSocial ?? inscrito.Cursista.Nome
                });
            }

            return lista;
        }
    }
}
