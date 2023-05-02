using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Exceptions;

namespace Trilhas.Data.Model.Eventos
{
    public class ListaDeInscricao : DefaultEntity
    {
        [ForeignKey("EventoId")]
        public Evento Evento { get; set; }

        public List<Inscrito> Inscritos { get; set; }

        public ListaDeInscricao()
        {
            Inscritos = new List<Inscrito>();
        }

        public int QuantidadeDeInscritos()
        {
            return Inscritos == null ? 0 : Inscritos.Count(x => !x.DeletionTime.HasValue);
        }

        public int QuantidadeDeInscritosPorEntidade(Entidade entidade)
        {
            return Inscritos == null ? 0 : Inscritos.Count(x => x.Cursista.Entidade.Id == entidade.Id && !x.DeletionTime.HasValue);
        }

        public bool EstaInscrito(Pessoa pessoa)
        {
            return Inscritos.Any(x => x.Cursista.Id == pessoa.Id && !x.DeletionTime.HasValue);
        }

        public List<Pessoa> PessoasInscritas()
        {
            return Inscritos.Where(x => !x.DeletionTime.HasValue).Select(x => x.Cursista).ToList();
        }

        public Inscrito RecuperarInscricao(Pessoa pessoa)
        {
            if (Inscritos != null)
            {
                return Inscritos.FirstOrDefault(x => x.Cursista.Id == pessoa.Id && !x.DeletionTime.HasValue);
            }

            return null;
        }

        public Inscrito Inscrever(Pessoa pessoa, DateTime dataInscricao)
        {
            if (Inscritos == null)
            {
                Inscritos = new List<Inscrito>();
            }

            //ValidarPeriodoDeInscricao();
            //ValidarSituacaoDoEvento();
            ValidarCursistaInscrito(pessoa);
            ValidarVagas();
            ValidarCotas(pessoa);

            Inscrito inscricao = new Inscrito
            {
                Cursista = pessoa,
                DataDeInscricao = dataInscricao
            };

            Inscritos.Add(inscricao);

            return inscricao;
        }

        public bool CancelarInscricao(Pessoa pessoa, string usuario)
        {
            var inscricao = RecuperarInscricao(pessoa);

            if (inscricao != null)
            {
                if (inscricao.Penalidade != null)
                {
                    inscricao.Penalidade.JustificativaDeCancelamento = "Inscrição Cancelada.";
                }

                inscricao.DeletionTime = DateTime.Now;
                inscricao.DeletionUserId = usuario;

                return true;
            }

            return false;
        }

        private void ValidarSituacaoDoEvento()
        {
            var situacao = Evento.Situacao();

            if (situacao == SituacaoEvento.Finalizado)
            {
                throw new TrilhasException("O Evento já está finalizado.");
            }
        }

        private void ValidarPeriodoDeInscricao()
        {
            var hoje = DateTime.Now;

            var agenda = Evento.Agenda();

            if (!(hoje >= agenda.DataHoraInscricaoInicio && hoje <= agenda.DataHoraInscricaoFim))
            {
                throw new TrilhasException("O Evento está fora do período de Inscrição.");
            }
        }

        private void ValidarCursistaInscrito(Pessoa pessoa)
        {
            if (EstaInscrito(pessoa))
            {
                throw new TrilhasException("O Cursista já está inscrito no Evento.");
            }
        }

        private void ValidarVagas()
        {
            if (QuantidadeDeInscritos() >= Evento.Agendas.Last().NumeroVagas)
            {
                throw new TrilhasException("O número de inscritos já atingiu quantidade de vagas do Evento.");
            }
        }

        private void ValidarCotas(Pessoa pessoa)
        {
            if (QuantidadeDeInscritosPorEntidade(pessoa.Entidade) >= CotaDaEntidadeDoCursista(pessoa))
            {
                throw new TrilhasException("As vagas destinadas à Entidade do Cursista já foram preenchidas.");
            }
        }

        private int CotaDaEntidadeDoCursista(Pessoa pessoa)
        {
            var cota = Evento.Cotas.FirstOrDefault(x => x.Entidade.Id == pessoa.Entidade.Id);

            if (cota != null)
            {
                return cota.Quantidade;
            }
            else if (Evento.LimitarInscricoes)
            {
                return Evento.VagasPorEntidade;
            }
            else
            {
                return Evento.Agenda().NumeroVagas;
            }
        }
    }
}
