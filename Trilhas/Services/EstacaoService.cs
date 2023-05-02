using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Model.Trilhas;

namespace Trilhas.Services
{
    public class EstacaoService
    {
        private ApplicationDbContext _context;
        private readonly SolucaoEducacionalService _solucaoEducacionalService;

        public EstacaoService(ApplicationDbContext context,
            SolucaoEducacionalService solucaoEducacionalService)
        {
            _context = context;
            _solucaoEducacionalService = solucaoEducacionalService;
        }

        public Estacao SalvarEstacao(string userId, Estacao estacao)
        {
            ValidarDuplicidade(estacao);

            if (estacao.Id > 0)
            {
                estacao.LastModifierUserId = userId;
                estacao.LastModificationTime = DateTime.Now;
            }
            else
            {
                estacao.CreatorUserId = userId;
                estacao.CreationTime = DateTime.Now;

                _context.Estacoes.Add(estacao);
            }

            _context.SaveChanges();

            return estacao;
        }

        private void ValidarDuplicidade(Estacao estacao)
        {
            if (_context.Estacoes.Include(x => x.Eixo)
                .Any(x => x.Nome == estacao.Nome && x.Eixo.Id == estacao.Eixo.Id && x.Id != estacao.Id && !x.DeletionTime.HasValue))
            {
                throw new TrilhasValidationException("Já existe uma Estação cadastrada com o mesmo nome neste Eixo.");
            }
        }

        public void ExcluirEstacao(string userId, long id)
        {
            var estacao = RecuperarEstacao(id);

            if (estacao == null)
            {
                throw new RecordNotFoundException("Registro não encontrado.");
            }

            if (!PodeExcluirEstacao(estacao))
            {
                throw new ConstraintException("Existem Soluções Educacionais vinculadas à Estação.");
            }

            estacao.DeletionTime = DateTime.Now;
            estacao.DeletionUserId = userId;

            _context.SaveChanges();
        }

        private bool PodeExcluirEstacao(Estacao estacao)
        {
            return _solucaoEducacionalService.PesquisarSolucaoEducacional(null, 0, estacao.Id).Count() == 0;
        }

        //public List<Estacao> RecupararEstacoes()
        //{
        //	List<Estacao> estacoes = _context.Estacoes
        //		.Include(x => x.Eixo)
        //		.Where(x => (!x.DeletionTime.HasValue))
        //		.ToList();
        //	return estacoes;
        //}

        public List<Estacao> RecupararEstacoesEixo(long eixoId)
        {
            List<Estacao> estacoes = _context.Estacoes
                .Include(x => x.Eixo)
                .Where(x => (x.Eixo.Id == eixoId) && (!x.DeletionTime.HasValue))
                .ToList();

            return estacoes;
        }

        public Estacao RecuperarEstacao(long id, bool incluirExcluidos = false)
        {
            return _context.Estacoes
                .Include(x => x.Eixo)
                .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));
        }

        public int QuantidadeDeEstacoes(string nome, long eixoId, bool excluidos)
        {
            return PesquisarEstacao(nome, eixoId, excluidos).Count();
        }

        public List<Estacao> PesquisarEstacoes(string nome, long eixoId, bool excluidos, int start = -1, int count = -1)
        {
            return PesquisarEstacao(nome, eixoId, excluidos, start, count).ToList();
        }

        private IQueryable<Estacao> PesquisarEstacao(string nome, long eixoId, bool exibirExcluidos, int start = -1, int count = -1)
        {
            IQueryable<Estacao> result = _context.Estacoes.Include(x => x.Eixo);

            if (!exibirExcluidos)
            {
                result = result.Where(x => !x.DeletionTime.HasValue);
            }
            if (!string.IsNullOrEmpty(nome))
            {
                result = result.Where(x => x.Nome.ToUpper().Contains(nome.Trim().ToUpper()));
            }
            if (eixoId > 0)
            {
                result = result.Where(x => x.Eixo.Id == eixoId);
            }

            result = result.OrderBy(x => x.Nome);

            if (start > 0)
            {
                result = result.Skip(start);
            }
            if (count > 0)
            {
                result = result.Take(count);
            }

            return result;
        }
    }
}
