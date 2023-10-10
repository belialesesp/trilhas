using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Trilhas.Data.Enums;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Certificados;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Model.Trilhas;

namespace Trilhas.Data.Model.Eventos
{
    public class Evento : DefaultEntity
    {
        [ForeignKey("CoordenadorPessoaId")]
        public Pessoa Coordenador { get; set; }
        [ForeignKey("CursoId")]
        public Curso Curso { get; set; }
        [ForeignKey("EntidadeDemandanteId")]
        public Entidade EntidadeDemandante { get; set; }
        [ForeignKey("LocalId")]
        public Local Local { get; set; }
        [ForeignKey("CertificadoId")]
        public Certificado Certificado { get; set; }
        [ForeignKey("DeclaracaoCursistaId")]
        public Certificado DeclaracaoCursista { get; set; }
        [ForeignKey("DeclaracaoDocenteId")]
        public Certificado DeclaracaoDocente { get; set; }

        public List<EventoCota> Cotas { get; set; }
        public List<EventoAgenda> Agendas { get; set; }
        public List<EventoRecurso> Recursos { get; set; }
        public List<EventoHorario> Horarios { get; set; }

        public bool LimitarInscricoes { get; set; }
        public int VagasPorEntidade { get; set; }
        public string Observacoes { get; set; }
        public string UrlEad { get; set; }
        public bool Finalizado { get; set; }
        public ListaDeInscricao ListaDeInscricao { get; set; }
        public string MotivoExclusao { get; set; }

        public SituacaoEvento Situacao()
        {
            if (DeletionTime.HasValue)
            {
                return SituacaoEvento.Cancelado;
            }
            else
            {
                EventoAgenda agenda = Agendas.Last();

                if (agenda.DataHoraInscricaoInicio <= DateTime.Now && agenda.DataHoraInscricaoFim >= DateTime.Now)
                {
                    return SituacaoEvento.Inscricao;
                }
                else if ((Agendas.Last().DataHoraInicio <= DateTime.Now) && (Agendas.Last().DataHoraFim >= DateTime.Now))
                {
                    return SituacaoEvento.Andamento;
                }
                else if (Agendas.Last().DataHoraInicio > DateTime.Now)
                {
                    return SituacaoEvento.Agendado;
                }
                else if (Agendas.Last().DataHoraFim < DateTime.Now)
                {
                    if (!Finalizado)
                    {
                        return SituacaoEvento.Encerrado;
                    }
                    else
                    {
                        return SituacaoEvento.Finalizado;
                    }
                }
                else
                {
                    return SituacaoEvento.Ativo;
                }
            }
        }

        public EventoAgenda Agenda()
        {
            if (Agendas == null)
            {
                return null;
            }

            return Agendas.Last();
        }

        public int QuantidadeDeVagas()
        {
            if (Agendas == null)
            {
                return 0;
            }

            return Agendas.Last().NumeroVagas;
        }

        public Inscrito InscreverCursista(Pessoa pessoa, DateTime dataInscricao)
        {
            if (ListaDeInscricao == null)
            {
                ListaDeInscricao = new ListaDeInscricao();
                ListaDeInscricao.Evento = this;
            }

            return ListaDeInscricao.Inscrever(pessoa, dataInscricao);
        }

        public bool CancelarInscricao(Pessoa pessoa, string usuario)
        {
            if (ListaDeInscricao == null)
            {
                return false;
            }

            return ListaDeInscricao.CancelarInscricao(pessoa, usuario);
        }

        public void Finalizar(int diasPenalidade)
        {
            if (Situacao() != SituacaoEvento.Encerrado)
            {
                throw new TrilhasException("O Evento ainda não está encerrado.");
            }

            CalcularFrequencias();
            GerarPenalidades(diasPenalidade);
            Finalizado = true;
        }

        public void CalcularFrequencias(long cursistaId = 0)
        {
            var inscritos = ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue && (cursistaId == 0 || x.Cursista.Id == cursistaId));

            foreach (var inscrito in inscritos)
            {
                double horasDeAula = 0;

                foreach (var horario in Horarios)
                {
                    if (horario.Presente(inscrito.Cursista))
                    {
                        horasDeAula += horario.TotalDeHoras();
                    }
                }

                inscrito.Frequencia = horasDeAula > 0 ? Math.Ceiling((horasDeAula / Curso.CargaHorariaTotal()) * 100) : 0;

                inscrito.Situacao = SituacaoCursista(inscrito.Frequencia);
            }
        }

        public EnumSituacaoCursista SituacaoCursista(double frequencia)
        {
            if (frequencia >= Curso.FrequenciaMinimaCertificado)
            {
                return EnumSituacaoCursista.CERTIFICADO;
            }
            else if (frequencia >= Curso.FrequenciaMinimaDeclaracao)
            {
                return EnumSituacaoCursista.DECLARADO;
            }
            else
            {
                return EnumSituacaoCursista.DESISTENTE;
            }
        }

        public void GerarPenalidades(int diasPenalidade)
        {
            var desistentes = ListaDeInscricao.Inscritos.Where(x => !x.DeletionTime.HasValue && x.Situacao == EnumSituacaoCursista.DESISTENTE);

            if (!desistentes.Any())
            {
                return;
            }

            DateTime dataFimEvento = Agenda().DataHoraFim;

            foreach (var desistente in desistentes)
            {
                desistente.Penalidade = new Penalidade(desistente.Cursista, dataFimEvento, diasPenalidade);
            }
        }
    }

    public enum SituacaoEvento
    {
        [Display(Name = "ATIVO")]
        Ativo,

        [Display(Name = "AGENDADO")]
        Agendado,

        [Display(Name = "INSCRIÇÃO")]
        Inscricao,

        [Display(Name = "ANDAMENTO")]
        Andamento,

        [Display(Name = "CANCELADO")]
        Cancelado,

        [Display(Name = "ENCERRADO")]
        Encerrado,

        [Display(Name = "FINALIZADO")]
        Finalizado
    }
}
