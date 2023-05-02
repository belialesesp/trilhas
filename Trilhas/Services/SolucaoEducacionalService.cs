using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Enums;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Model.Trilhas;

namespace Trilhas.Services
{
    public class SolucaoEducacionalService
    {
        private ApplicationDbContext _context;
        private MinioService _minioService;

        public SolucaoEducacionalService(ApplicationDbContext context,
            MinioService minioService)
        {
            _context = context;
            _minioService = minioService;
        }

        //SOLUCÃO EDUCACIONAL
        public SolucaoEducacional SalvarSolucaoEducacional(string userId, SolucaoEducacional solucao)
        {
            if (solucao.GetType() == typeof(Curso))
            {
                ValidarDuplicidade((Curso)solucao);
            }

            if (solucao.Id > 0)
            {
                if (solucao.TipoDeSolucao.Equals("curso"))
                {
                    ValidarPossibilidadeDeEdicao((Curso)solucao);

                    _context.Habilidades.RemoveRange(_context.Habilidades.Include(x => x.Curso).Where(x => x.Curso.Id == solucao.Id));
                    _context.Modulos.RemoveRange(_context.Modulos.Include(x => x.Curso).Where(x => x.Curso.Id == solucao.Id));
                }

                solucao.LastModifierUserId = userId;
                solucao.LastModificationTime = DateTime.Now;
            }
            else
            {
                solucao.CreatorUserId = userId;
                solucao.CreationTime = DateTime.Now;

                _context.SolucoesEducacionais.Add(solucao);
            }

            _context.SaveChanges();

            return solucao;
        }

        private void ValidarDuplicidade(Curso curso)
        {
            if (_context.SolucoesEducacionais.OfType<Curso>().Any(x => x.Sigla == curso.Sigla && x.Id != curso.Id && !x.DeletionTime.HasValue))
            {
                throw new TrilhasValidationException("Já existe um Curso cadastrado com esta sigla.");
            }
        }

        private void ValidarPossibilidadeDeEdicao(Curso curso)
        {
            var possuiInscritos = _context.Eventos
                .Include(x => x.Curso)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Inscritos)
                .Any(x => x.Curso.Id == curso.Id && !x.Curso.DeletionTime.HasValue && x.ListaDeInscricao.Inscritos.Any(y => !y.DeletionTime.HasValue));

            if (possuiInscritos)
            {
                throw new TrilhasException("Existem pessoas inscritas em Eventos deste Curso.");
            }
        }

        private bool PodeExcluirSolucao(SolucaoEducacional solucao)
        {
            return !_context.Eventos
                .Include(x => x.Curso)
                .Any(x => x.Curso.Id == solucao.Id && !x.DeletionTime.HasValue);
        }

        public void ExcluirSolucaoEducacional(string userId, long id)
        {
            SolucaoEducacional solucaoEducacional = RecuperarSolucaoEducacional(id);

            if (solucaoEducacional == null)
            {
                throw new RecordNotFoundException("Registro não encontrado.");
            }

            if (!PodeExcluirSolucao(solucaoEducacional))
            {
                throw new ConstraintException("Existem Eventos vinculados a esta Solução.");
            }

            solucaoEducacional.DeletionTime = DateTime.Now;
            solucaoEducacional.DeletionUserId = userId;

            _context.SaveChanges();
        }

        public SolucaoEducacional RecuperarSolucaoEducacional(long id, bool incluirExcluidos = false)
        {
            return _context.SolucoesEducacionais
                .Include(x => x.Estacao)
                .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));
        }

        public SolucaoEducacional RecuperarSolucaoEducacionalCompleta(long id, bool incluirExcluidos = false)
        {
            var solucao = _context.SolucoesEducacionais
                .Include(x => x.Estacao)
                .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));

            if (solucao != null && solucao.TipoDeSolucao.ToLower().Equals("curso"))
            {
                return _context.SolucoesEducacionais.OfType<Curso>()
                .Include(x => x.TipoDoCurso)
                .Include(x => x.NivelDoCurso)
                .Include(x => x.Habilidades)
                .Include(x => x.Modulos)
                //.Include(x => x.Modalidade)
                .FirstOrDefault(x => x.Id == id);
            }

            return solucao;
        }

        public List<SolucaoEducacional> RecuperarSolucoesEducacionais(long estacaoId)
        {
            List<SolucaoEducacional> solucoes = _context.SolucoesEducacionais
                .Include(x => x.Estacao)
                .Where(x => x.Estacao.Id == estacaoId && (!x.DeletionTime.HasValue))
                .ToList();

            return solucoes;
        }

        public int QuantidadeDeSolucoesEducacionais(EnumModalidade? modalidadeCurso, long eixoId = 0, long estacaoId = 0, string titulo = null, string tipoSolucao = null, string autor = null, string editora = null, string responsavel = null, string sigla = null, long nivelCurso = 0, bool excluidos = false)
        {
            return PesquisarSolucaoEducacional(modalidadeCurso, eixoId, estacaoId, titulo, tipoSolucao, autor, editora, responsavel, sigla, nivelCurso, excluidos).Count();
        }

        public List<SolucaoEducacional> PesquisarSolucoesEducacionais(EnumModalidade? modalidadeCurso, long eixoId = 0, long estacaoId = 0, string titulo = null, string tipoSolucao = null, string autor = null, string editora = null, string responsavel = null, string sigla = null, long nivelCurso = 0, bool excluidos = false, int start = -1, int count = -1)
        {
            return PesquisarSolucaoEducacional(modalidadeCurso, eixoId, estacaoId, titulo, tipoSolucao, autor, editora, responsavel, sigla, nivelCurso, excluidos, start, count).ToList();
        }

        public IQueryable<SolucaoEducacional> PesquisarSolucaoEducacional(EnumModalidade? modalidadeCurso, long eixoId = 0, long estacaoId = 0, string titulo = null, string tipoSolucao = null, string autor = null, string editora = null, string responsavel = null, string sigla = null, long nivelCurso = 0, bool excluidos = false, int start = -1, int count = -1)
        {
            IQueryable<SolucaoEducacional> result = _context.SolucoesEducacionais
                .Include(x => x.Estacao.Eixo);

            if (!excluidos)
            {
                result = result.Where(x => !x.DeletionTime.HasValue);
            }
            if (!string.IsNullOrEmpty(titulo))
            {
                titulo = titulo.Trim().ToUpper();
                result = result.OfType<Curso>().Where(x => (x.TipoDeSolucao.Equals("curso") && x.Titulo.ToUpper().Contains(titulo) || x.Sigla.ToUpper().Contains(titulo)) || x.Titulo.ToUpper().Contains(titulo));
            }
            if (eixoId > 0)
            {
                result = result.Where(x => x.Estacao.Eixo.Id == eixoId);
            }
            if (estacaoId > 0)
            {
                result = result.Where(x => x.Estacao.Id == estacaoId);
            }

            if (!string.IsNullOrEmpty(tipoSolucao))
            {
                if (tipoSolucao.ToLower().Equals("curso"))
                {
                    result = result.OfType<Curso>()
                        .Include(x => x.TipoDoCurso)
                        .Include(x => x.NivelDoCurso)
                        .Include(x => x.Habilidades)
                        .Include(x => x.Modulos);

                    if (!string.IsNullOrEmpty(sigla))
                    {
                        result = result.OfType<Curso>().Where(x => x.Sigla.ToUpper().Contains(sigla.Trim().ToUpper()));
                    }
                    if (nivelCurso > 0)
                    {
                        result = result.OfType<Curso>().Where(x => x.NivelDoCurso.Id == nivelCurso);
                    }
                    if (modalidadeCurso.HasValue)
                    {
                        result = result.OfType<Curso>().Where(x => x.Modalidade == modalidadeCurso);
                    }
                }
                else if (tipoSolucao.ToLower().Equals("livro"))
                {
                    result = result.OfType<Livro>();

                    if (!string.IsNullOrEmpty(autor))
                    {
                        result = result.OfType<Livro>().Where(x => x.Autor.ToUpper().Contains(autor.Trim().ToUpper()));
                    }
                    if (!string.IsNullOrEmpty(editora))
                    {
                        result = result.OfType<Livro>().Where(x => x.Editora.ToUpper().Contains(editora.Trim().ToUpper()));
                    }
                }
                else if (tipoSolucao.ToLower().Equals("video"))
                {
                    result = result.OfType<Video>();

                    if (!string.IsNullOrEmpty(responsavel))
                    {
                        result = result.OfType<Video>().Where(x => x.Responsavel.ToUpper().Contains(responsavel.Trim().ToUpper()));
                    }
                }
            }

            result = result.OrderBy(x => x.Titulo);

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

        public TipoDeCurso RecuperarTipoDeCurso(long tipoCursoId)
        {
            return _context.TiposDeCurso.FirstOrDefault(x => x.Id == tipoCursoId);
        }

        public List<TipoDeCurso> RecuperarTiposDeCurso()
        {
            return _context.TiposDeCurso.ToList();
        }

        public NivelDeCurso RecuperarNivelDeCurso(long nivelCursoId)
        {
            return _context.NiveisDeCurso.FirstOrDefault(x => x.Id == nivelCursoId);
        }

        public List<NivelDeCurso> RecuperarNiveisDeCurso()
        {
            return _context.NiveisDeCurso.ToList();
        }

        public Modulo RecuperarModulo(long moduloId)
        {
            return _context.Modulos.FirstOrDefault(x => x.Id == moduloId);
        }
    }
}
