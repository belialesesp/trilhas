using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Exceptions;

namespace Trilhas.Services
{
    public class EntidadeService
    {
        private ApplicationDbContext _context;

        public EntidadeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Entidade SalvarEntidade(string userId, Entidade entidade)
        {
            ValidarDuplicidade(entidade);

            if (entidade.Id > 0)
            {
                entidade.LastModifierUserId = userId;
                entidade.LastModificationTime = DateTime.Now;

                _context.Gestores.RemoveRange(_context.Gestores.Include(x => x.Entidade).Where(x => x.Entidade.Id == entidade.Id));
            }
            else
            {
                entidade.CreatorUserId = userId;
                entidade.CreationTime = DateTime.Now;

                _context.Entidades.Add(entidade);
            }

            _context.SaveChanges();

            return entidade;
        }

        private void ValidarDuplicidade(Entidade entidade)
        {
            if (_context.Entidades.Any(x => x.Sigla == entidade.Sigla && x.Id != entidade.Id && !x.DeletionTime.HasValue))
            {
                throw new TrilhasValidationException("Já existe uma Entidade cadastrada com esta sigla.");
            }
        }

        public void ExcluirEntidade(string userId, long id)
        {
            Entidade entidade = RecuperarEntidade(id);

            if (entidade == null)
            {
                throw new RecordNotFoundException("Registro não encontrado.");
            }

            if (ExistemEventosVinculados(entidade))
            {
                throw new ConstraintException("Existem Soluções Educacionais vinculadas à Entidade.");
            }
            else if (ExistemServidoresVinculados(entidade))
            {
                throw new ConstraintException("Existem Servidores vinculados à Entidade.");
            }

            entidade.DeletionTime = DateTime.Now;
            entidade.DeletionUserId = userId;

            _context.SaveChanges();
        }

        private bool ExistemEventosVinculados(Entidade entidade)
        {
            return _context.Eventos
                .Include(x => x.EntidadeDemandante)
                .Any(x => x.EntidadeDemandante.Id == entidade.Id && !x.DeletionTime.HasValue);
        }

        private bool ExistemServidoresVinculados(Entidade entidade)
        {
            return _context.Pessoas
                .Include(x => x.Entidade)
                .Any(x => x.Entidade.Id == entidade.Id && !x.DeletionTime.HasValue);
        }

        public Entidade RecuperarEntidade(long id, bool incluirExcluidos = false)
        {
            return _context.Entidades.FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));
        }

        public Entidade RecuperarEntidadeCompleta(long id, bool incluirExcluidos = false)
        {
            return _context.Entidades
                .Include(x => x.Municipio)
                .Include(x => x.Tipo)
                .Include(x => x.Gestores).ThenInclude(x => x.Pessoa)
                .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));
        }

        public int QuantidadeDeEntidades(string nome, long tipoEntidadeId, string uf, long municipioId, bool excluidos)
        {
            return PesquisarEntidade(nome, tipoEntidadeId, uf, municipioId, excluidos).Count();
        }

        public List<Entidade> PesquisarEntidades(string nome, long tipoEntidadeId, string uf, long municipioId, bool excluidos, int start = -1, int count = -1)
        {
            return PesquisarEntidade(nome, tipoEntidadeId, uf, municipioId, excluidos, start, count).ToList();
        }

        private IQueryable<Entidade> PesquisarEntidade(string nome, long tipoEntidadeId, string uf, long municipioId, bool exibirExcluidos, int start = -1, int count = -1)
        {
            IQueryable<Entidade> result = _context.Entidades
            .Include(x => x.Municipio)
            .Include(x => x.Tipo);

            if (!exibirExcluidos)
            {
                result = result.Where(x => !x.DeletionTime.HasValue);
            }
            if (!string.IsNullOrEmpty(nome))
            {
                nome = nome.Trim().ToUpper();
                result = result.Where(x => x.Sigla.ToUpper().Contains(nome) || x.Nome.ToUpper().Contains(nome));
            }
            if (tipoEntidadeId > 0)
            {
                result = result.Where(x => x.Tipo.Id == tipoEntidadeId);
            }
            if (!string.IsNullOrEmpty(uf))
            {
                result = result.Where(x => x.Municipio.Uf.ToUpper().Contains(uf.Trim().ToUpper()));
            }
            if (municipioId > 0)
            {
                result = result.Where(x => x.Municipio.Id == municipioId);
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

        public TipoDeEntidade RecuperarTipoDeEntidade(long id)
        {
            return _context.TiposDeEntidade.Find(id);
        }

        public List<TipoDeEntidade> RecuperarTipoEntidade()
        {
            return _context.TiposDeEntidade.ToList();
        }
    }
}
