using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Exceptions;

namespace Trilhas.Services
{
    public class RecursoService
    {
        private ApplicationDbContext _context;

        public RecursoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Recurso SalvarRecurso(string userId, Recurso recurso)
        {
            ValidarDuplicidade(recurso);

            if (recurso.Id > 0)
            {
                recurso.LastModifierUserId = userId;
                recurso.LastModificationTime = DateTime.Now;
            }
            else
            {
                recurso.CreatorUserId = userId;
                recurso.CreationTime = DateTime.Now;

                _context.Recursos.Add(recurso);
            }

            _context.SaveChanges();

            return recurso;
        }

        private void ValidarDuplicidade(Recurso recurso)
        {
            if (_context.Recursos.Any(x => x.Nome == recurso.Nome && x.Id != recurso.Id && !x.DeletionTime.HasValue))
            {
                throw new TrilhasValidationException("Já existe um Recurso cadastrado com o mesmo nome.");
            }
        }

        public void ExcluirRecurso(string userId, long id)
        {
            Recurso recurso = RecuperarRecurso(id);

            if (recurso == null)
            {
                throw new RecordNotFoundException("Registro não encontrado.");
            }

            if (!ExistemEventosVinculados(recurso))
            {
                throw new ConstraintException("O Recurso está vinculado a algum Evento.");
            }

            if (!ExistemLocaisVinculados(recurso))
            {
                throw new ConstraintException("O Recurso está vinculado a algum Local.");
            }

            recurso.DeletionTime = DateTime.Now;
            recurso.DeletionUserId = userId;

            _context.SaveChanges();
        }

        private bool ExistemEventosVinculados(Recurso recurso)
        {
            return !_context.EventoRecurso.Include(x => x.Recurso)
                .Any(x => x.Recurso.Id == recurso.Id && !x.Evento.DeletionTime.HasValue);
        }

        private bool ExistemLocaisVinculados(Recurso recurso)
        {
            return !_context.LocalRecursos.Include(x => x.Recurso)
                .Any(x => x.Recurso.Id == recurso.Id && !x.DeletionTime.HasValue);
        }

        public Recurso RecuperarRecurso(long id, bool incluirExcluidos = false)
        {
            return _context.Recursos
                .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));
        }

        public int QuantidadeDeRecursos(string nome, string descricao, bool excluidos)
        {
            return PesquisarRecurso(nome, descricao, excluidos).Count();
        }

        public List<Recurso> PesquisarRecursos(string nome, string descricao, bool excluidos, int start = -1, int count = -1)
        {
            return PesquisarRecurso(nome, descricao, excluidos, start, count).ToList();
        }

        private IQueryable<Recurso> PesquisarRecurso(string nome, string descricao, bool exibirExcluidos, int start = -1, int count = -1)
        {
            IQueryable<Recurso> result = _context.Recursos;

            if (!exibirExcluidos)
            {
                result = result.Where(x => !x.DeletionTime.HasValue);
            }
            if (!string.IsNullOrEmpty(nome))
            {
                result = result.Where(x => x.Nome.ToUpper().Contains(nome.Trim().ToUpper()));
            }
            if (!string.IsNullOrEmpty(descricao))
            {
                result = result.Where(x => x.Descricao.ToUpper().Contains(descricao.Trim().ToUpper()));
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
