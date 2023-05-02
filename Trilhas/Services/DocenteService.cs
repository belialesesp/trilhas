using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Exceptions;
using Trilhas.SqlDto;

namespace Trilhas.Services
{
    public class DocenteService
    {
        private ApplicationDbContext _context;

        public DocenteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int QuantidadeDeDocentes(string nome, string email, string cpf, string numeroFuncional, bool exibirExcluidos)
        {
            return PesquisarDocente(nome, email, cpf, numeroFuncional, exibirExcluidos).Count();
        }

        public List<Docente> PesquisarDocentes(string nome, string email, string cpf, string numeroFuncional, bool excluidos, int start = -1, int count = -1)
        {
            return PesquisarDocente(nome, email, cpf, numeroFuncional, excluidos, start, count).ToList();
        }

        private IQueryable<Docente> PesquisarDocente(string nome, string email, string cpf, string numeroFuncional, bool exibirExcluidos, int start = -1, int count = -1)
        {
            IQueryable<Docente> result = _context.Docentes.Include(x => x.Pessoa);

            if (!exibirExcluidos)
            {
                result = result.Where(x => !x.DeletionTime.HasValue);
            }
            if (!string.IsNullOrEmpty(nome))
            {
                result = result.Where(x => x.Pessoa.Nome.ToUpper().Contains(nome.Trim().ToUpper()) || x.Pessoa.NomeSocial.ToUpper().Contains(nome.Trim().ToUpper()));
            }
            if (!string.IsNullOrEmpty(email))
            {
                result = result.Where(x => x.Pessoa.Email.ToUpper().Contains(email.Trim().ToUpper()));
            }
            if (!string.IsNullOrEmpty(cpf))
            {
                result = result.Where(x => x.Pessoa.Cpf.ToUpper().Contains(cpf.Trim().ToUpper()));
            }
            if (!string.IsNullOrEmpty(numeroFuncional))
            {
                result = result.Where(x => x.Pessoa.NumeroFuncional.ToUpper().Contains(numeroFuncional.Trim().ToUpper()));
            }

            result = result.OrderBy(x => x.Pessoa.Nome);

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

        public Docente RecuperarDocente(long id, bool incluirExcluidos = false)
        {
            Docente docente = _context.Docentes
                .Include(x => x.Pessoa)
                .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));

            return docente;
        }

        public Docente RecuperarDocenteCompleto(long id, bool incluirExcluidos = false)
        {
            Docente docente = _context.Docentes
                .Include(x => x.Habilitacao).ThenInclude(x => x.Curso).ThenInclude(x => x.Modulos)
                .Include(x => x.Habilitacao).ThenInclude(x => x.Curso)
                .Include(x => x.Formacao)
                .Include(x => x.DadosBancarios)
                .Include(x => x.Pessoa)
                .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));

            return docente;
        }

        public Docente SalvarDocente(string userId, Docente docente)
        {
            ValidarDuplicidade(docente);

            RemoverHabilitacoesDuplicadas(docente);

            if (docente.Id > 0)
            {
                _context.Habilitacao.RemoveRange(_context.Habilitacao.Include(x => x.Docente).Where(x => x.Docente.Id == docente.Id && !docente.Habilitacao.Any(y => y.Id == x.Id)));
                _context.Formacao.RemoveRange(_context.Formacao.Include(x => x.Docente).Where(x => x.Docente.Id == docente.Id && !docente.Formacao.Any(y => y.Id == x.Id)));

                docente.LastModifierUserId = userId;
                docente.LastModificationTime = DateTime.Now;
            }
            else
            {
                docente.CreatorUserId = userId;
                docente.CreationTime = DateTime.Now;

                _context.Docentes.Add(docente);
            }

            _context.SaveChanges();

            return docente;
        }

        private void ValidarDuplicidade(Docente docente)
        {
            if (_context.Docentes.Include(x => x.Pessoa).Any(x => x.Pessoa.Cpf == docente.Pessoa.Cpf && x.Id != docente.Id && !x.DeletionTime.HasValue))
            {
                throw new TrilhasValidationException("Já existe um cadastro deste Docente.");
            }
        }

        private void RemoverHabilitacoesDuplicadas(Docente docente)
        {
            Habilitacao h1, h2;

            for (var x = 0; x < (docente.Habilitacao.Count - 1) || docente.Habilitacao == null; x++)
            {
                h1 = docente.Habilitacao[x];

                for (var y = (x + 1); y < docente.Habilitacao.Count; y++)
                {
                    h2 = docente.Habilitacao[y];

                    if (h1.Curso.Id == h2.Curso.Id)
                    {
                        docente.Habilitacao.Remove(h2);
                        y--;
                    }
                }
            }
        }

        public void ExcluirDocente(string userId, long id)
        {
            Docente docente = RecuperarDocente(id);

            if (docente == null)
            {
                throw new RecordNotFoundException("Registro não encontrado.");
            }

            if (!PodeExcluirDocente(docente))
            {
                throw new ConstraintException("Existem alguma entidade vinculadas à Docente.");
            }

            docente.DeletionTime = DateTime.Now;
            docente.DeletionUserId = userId;

            _context.SaveChanges();
        }

        private bool PodeExcluirDocente(Docente docente)
        {
            return !_context.Eventos
                 .Include(x => x.Horarios).ThenInclude(x => x.Docente)
                 .Any(x => x.Horarios.Any(y => y.Docente.Id == docente.Id) && !x.DeletionTime.HasValue);
        }

        public Habilitacao RecuperarHabilitacao(long id)
        {
            return _context.Habilitacao.FirstOrDefault(x => x.Id == id);
        }

        public Formacao RecuperarFormacao(long id)
        {
            return _context.Formacao.FirstOrDefault(x => x.Id == id);
        }

        public DadosBancarios RecuperarDadosBancarios(long id)
        {
            return _context.DadosBancarios
                .FirstOrDefault(x => x.Id == id);
        }

        public int PesquisarQuantidadeDocenteSqlQuery(string nomeDocente, long cursoId, int? modalidadeCurso, DateTime? dataInicio, DateTime? dataFim, bool excluidos = false, int start = -1, int count = -1)
        {
            return QueryDocentes(nomeDocente, cursoId, modalidadeCurso, dataInicio, dataFim, excluidos).Count();
        }

        public List<GridDocenteDto> PesquisarDocenteSqlQuery(string nomeDocente, long cursoId, int? modalidadeCurso, DateTime? dataInicio, DateTime? dataFim, bool excluidos = false, int start = -1, int count = -1)
        {
            var query = QueryDocentes(nomeDocente, cursoId, modalidadeCurso, dataInicio, dataFim, excluidos);

            if (start > 0)
            {
                query = query.Skip(start);
            }

            if (count > 0)
            {
                query = query.Take(count);
            }

            return query.ToList();
        }

        public IQueryable<GridDocenteDto> QueryDocentes(string nomeDocente, long cursoId, int? modalidadeCurso, DateTime? dataInicio, DateTime? dataFim, bool excluidos)
        {
            SqlParameter docente = new SqlParameter("@nomeDocente", "%" + nomeDocente + "%");
            SqlParameter exibirExcluidos = new SqlParameter("@exibirExcluidos", Convert.ToInt32(excluidos));
            SqlParameter curso = new SqlParameter("@cursoId", cursoId);
            SqlParameter dtInicio = new SqlParameter("@dataInicio", dataInicio.HasValue ? dataInicio.Value.ToString("yyyy-MM-dd") : "");
            SqlParameter dtFim = new SqlParameter("@dataFim", dataFim.HasValue ? dataFim.Value.ToString("yyyy-MM-dd") : "");
            SqlParameter modalidade = new SqlParameter("@modalidade", modalidadeCurso.HasValue ? modalidadeCurso.Value.ToString() : "");

            string sql = $@"select f.id, f.nome, f.cpf, f.email, count(f.QuantidadeEvento) as QuantidadeEvento, sum(f.cargahorariatotal) as CargaHorariaTotal, f.Excluido from 
                                (select d.Id, coalesce(p.NomeSocial, p.Nome) as Nome, p.cpf, p.Email, e.Id as QuantidadeEvento, 
                                    (select coalesce(sum(s.CargaHoraria), 0) from (select m.CargaHoraria as CargaHoraria
		                                    from modulos m join EventoHorario h on (m.id = h.ModuloId)
		                                    where h.EventoId = e.Id
                                                and (@dataInicio = '' or h.DataHoraInicio >= @dataInicio)
                                                and (@dataFim = '' or h.DataHoraFim <= @dataFim)
		                                    group by m.id, m.CargaHoraria
                                    ) as s) as CargaHorariaTotal,
                                        case  when d.deletionTime is null then 0 else 1 end as Excluido
                                    from dbo.Docentes d
                                    left join dbo.Pessoas p on d.PessoaId = p.Id
                                    left join dbo.Habilitacao b on b.DocenteId = d.Id
                                    left join dbo.EventoHorario eh on d.Id = eh.DocenteId
                                    left join dbo.Eventos e on eh.EventoId = e.Id
                                    left join dbo.SolucoesEducacionais se on se.Id = e.CursoId
                                where e.DeletionTime is null
                                    and (p.Nome like @nomeDocente or p.NomeSocial like @nomeDocente)
                                    and (@exibirExcluidos = 1 or d.DeletionTime is null)
                                    and (@cursoId = 0 or (b.cursoId = @cursoId or se.id = @cursoId))
                                    and (@modalidade = '' or se.modalidade = @modalidade)
                                group by d.Id, p.NomeSocial, p.Nome, p.cpf, p.Email, d.deletionTime, e.id) as f
                            group by f.id, f.Nome, f.cpf, f.email, f.Excluido";

            return _context.Query<GridDocenteDto>().FromSql(sql, docente, exibirExcluidos, curso, dtInicio, dtFim, modalidade);
        }
    }
}
