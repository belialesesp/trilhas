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
    public class PessoaService
    {
        private ApplicationDbContext _context;
        private MinioService _minioService;

        public PessoaService(ApplicationDbContext context,
                MinioService minioService)
        {
            _context = context;
            _minioService = minioService;
        }

        public int QuantidadeDePessoas(string nome, string email, string cpf, string numeroFuncional, long entidadeId, long gestorId, bool excluidos)
        {
            return PesquisarPessoa(nome, email, cpf, numeroFuncional, entidadeId, gestorId, excluidos).Count();
        }

        public List<Pessoa> PesquisarPessoas(string nome, string email, string cpf, string numeroFuncional, long entidadeId, long gestorId, bool excluidos, int start = -1, int count = -1)
        {
            return PesquisarPessoa(nome, email, cpf, numeroFuncional, entidadeId, gestorId, excluidos, start, count).ToList();
        }

        private IQueryable<Pessoa> PesquisarPessoa(string nome, string email, string cpf, string numeroFuncional, long entidadeId, long gestorId, bool exibirExcluidos, int start = -1, int count = -1)
        {
            IQueryable<Pessoa> result = _context.Pessoas
                .Include(x => x.Municipio)
                .Include(x => x.Escolaridade)
                .Include(x => x.Deficiencia)
                .Include(x => x.Contatos)
                .Include(x => x.Entidade).ThenInclude(x => x.Gestores);

            if (!exibirExcluidos)
            {
                result = result.Where(x => !x.DeletionTime.HasValue);
            }
            if (!string.IsNullOrEmpty(nome))
            {
                result = result.Where(x => x.Nome.ToUpper().Contains(nome.Trim().ToUpper()) || x.NomeSocial.ToUpper().Contains(nome.Trim().ToUpper()));
            }
            if (!string.IsNullOrEmpty(cpf))
            {
                result = result.Where(x => x.Cpf.ToUpper().Contains(cpf.Trim().ToUpper()));
            }
            if (!string.IsNullOrEmpty(numeroFuncional))
            {
                result = result.Where(x => x.NumeroFuncional.ToUpper().Contains(numeroFuncional.Trim().ToUpper()));
            }
            if (!string.IsNullOrEmpty(email))
            {
                result = result.Where(x => x.Email.ToUpper().Contains(email.Trim().ToUpper()));
            }
            if (entidadeId > 0)
            {
                result = result.Where(x => x.Entidade.Id == entidadeId);
            }
            if (gestorId > 0)
            {
                result = result.Where(x => x.Entidade.Gestores.Any(y => y.Id == gestorId));
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

        public Pessoa RecuperarPessoa(long id, bool incluirExcluidos = false)
        {
            return _context.Pessoas
                .Include(x => x.Entidade)
                .FirstOrDefault(x => x.Id == id);
        }

        public Pessoa RecuperarPessoaCompleto(long id, bool incluirImagem = false, bool incluirExcluidos = false)
        {
            Pessoa pessoa = _context.Pessoas
                .Include(x => x.Municipio)
                .Include(x => x.OrgaoExpedidorIdentidade)
                .Include(x => x.Entidade)
                .Include(x => x.Sexo)
                .Include(x => x.Escolaridade)
                .Include(x => x.Contatos).ThenInclude(a => a.TipoContato)
                .Include(x => x.Deficiencia)
                .FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));

            if (pessoa != null && incluirImagem)
            {
                pessoa.Imagem = RecuperarImagemPessoa(pessoa.Id);
            }

            return pessoa;
        }

        public List<Pessoa> RecuperarPessoas(List<long> ids, bool incluirImagem = false)
        {
            var pessoas = _context.Pessoas.Where(x => ids.Contains(x.Id) && (!x.DeletionTime.HasValue)).ToList();

            if (pessoas != null && incluirImagem)
            {
                foreach (var pessoa in pessoas)
                {
                    pessoa.Imagem = RecuperarImagemPessoa(pessoa.Id);
                }
            }

            return pessoas;
        }

        public Pessoa RecuperarPessoaPorCpf(string cpf)
        {
            return _context.Pessoas.FirstOrDefault(x => x.Cpf == cpf && !x.DeletionTime.HasValue);
        }

        public Pessoa SalvarPessoa(string userId, Pessoa pessoa)
        {
            var tx = _context.Database.BeginTransaction();

            try
            {
                ValidarDuplicidade(pessoa);

                if (string.IsNullOrEmpty(pessoa.NomeSocial))
                {
                    pessoa.NomeSocial = null;
                }

                if (pessoa.Id > 0)
                {
                    pessoa.LastModifierUserId = userId;
                    pessoa.LastModificationTime = DateTime.Now;
                }
                else
                {
                    pessoa.CreatorUserId = userId;
                    pessoa.CreationTime = DateTime.Now;

                    _context.Pessoas.Add(pessoa);
                }

                _context.SaveChanges();

                if (pessoa.Imagem == null)
                {
                    _minioService.ExcluirImagemPessoa("pessoa-" + pessoa.Id);
                }
                else
                {
                    Arquivo imagem = new Arquivo
                    {
                        Nome = "pessoa-" + pessoa.Id
                    };
                    imagem.FromBase64(pessoa.Imagem);

                    _minioService.SalvarImagemPessoa(imagem);
                }

                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }

            return pessoa;
        }

        private void ValidarDuplicidade(Pessoa pessoa)
        {
            if (_context.Pessoas.Any(x => x.Cpf == pessoa.Cpf && x.Id != pessoa.Id && !x.DeletionTime.HasValue))
            {
                throw new TrilhasValidationException("Já existe uma pessoa cadastrada com o mesmo CPf.");
            }
            if (_context.Pessoas.Any(x => !string.IsNullOrEmpty(x.NumeroFuncional) && x.NumeroFuncional == pessoa.NumeroFuncional && x.Id != pessoa.Id && !x.DeletionTime.HasValue))
            {
                throw new TrilhasValidationException("Já existe uma pessoa cadastrada com o mesmo numero funcional.");
            }
        }

        public void ExcluirPessoa(string userId, long id)
        {
            Pessoa pessoa = RecuperarPessoaCompleto(id);

            if (pessoa == null)
            {
                throw new RecordNotFoundException("Registro não encontrado.");
            }

            if (!PodeExcluirPessoa(pessoa))
            {
                throw new ConstraintException("Existem alguma entidade vinculadas à Pessoa.");
            }

            _minioService.ExcluirImagemPessoa("pessoa-" + id);

            pessoa.DeletionTime = DateTime.Now;
            pessoa.DeletionUserId = userId;

            _context.SaveChanges();
        }

        private bool PodeExcluirPessoa(Pessoa pessoa)
        {
            // CURSISTA
            bool condicao1 = !_context.Inscritos
                .Include(x => x.Cursista)
                .Any(x => x.Cursista.Id == pessoa.Id && !x.DeletionTime.HasValue);

            // GESTOR
            bool condicao2 = condicao1 && !_context.Entidades
                .Include(x => x.Gestores).ThenInclude(x => x.Pessoa)
                .Any(x => x.Gestores.Any(y => y.Pessoa.Id == pessoa.Id) && !x.DeletionTime.HasValue);

            // DOCENTE
            bool condicao3 = condicao2 && !_context.Docentes
                .Include(x => x.Pessoa)
                .Any(x => x.Pessoa.Id == pessoa.Id && !x.DeletionTime.HasValue);

            return condicao3;
        }

        public Deficiencia RecuperarDeficiencia(long deficienciaId)
        {
            Deficiencia deficiencia = _context.Deficiencias
                .Where(x => x.Id == deficienciaId)
                .FirstOrDefault();

            return deficiencia;
        }

        public Escolaridade RecuperarEscolaridade(long escolaridadeId)
        {
            Escolaridade escolaridade = _context.Escolaridades
                .Where(x => x.Id == escolaridadeId)
                .FirstOrDefault();
            return escolaridade;
        }

        public List<Escolaridade> RecuperarEscolaridadeAll()
        {
            return _context.Escolaridades.OrderBy(x => x.Nome).ToList();
        }

        public List<Deficiencia> RecuperarDeficienciaAll()
        {
            return _context.Deficiencias.OrderBy(x => x.Nome).ToList();
        }

        public List<OrgaoExpedidor> RecuperarOrgaoExpedidorAll()
        {
            return _context.OrgaoExpedidor.OrderBy(x => x.Nome).ToList();
        }

        public OrgaoExpedidor RecuperarOrgaoExpedidor(long orgaoExpedidorId)
        {
            OrgaoExpedidor orgaoExpedidor = _context.OrgaoExpedidor
                .Where(x => x.Id == orgaoExpedidorId)
                .FirstOrDefault();
            return orgaoExpedidor;
        }

        public string RecuperarImagemPessoa(long pessoaId)
        {
            Arquivo arquivo = _minioService.RecuperarImagemPessoa("pessoa-" + pessoaId).Result;

            if (arquivo != null)
            {
                return arquivo.ArquivoBase64;
            }

            return null;
        }

        public List<Sexo> RecuperarSexo()
        {
            return _context.Sexo.ToList();
        }

        public Sexo RecuperarSexo(long sexoId)
        {
            return _context.Sexo
                .Where(x => x.Id == sexoId)
                .FirstOrDefault();
        }

        public List<TipoPessoaContato> RecuperarTipoPessoaContatoAll()
        {
            return _context.TipoPessoaContato.ToList();
        }

        public PessoaContato RecuperarPessoaContato(long idPessoaContato)
        {
            return _context.PessoaContato
                .Include(x => x.TipoContato)
                //.Include(x => x.Pessoa)
                .Where(x => x.Id == idPessoaContato)
                .FirstOrDefault();
        }

        public int PesquisarQuantidadeCursistasSqlQuery(long? cursistaId, long? cursoId, long? modalidadeCurso, long? entidadeId, string ufNome, long? municipioId, DateTime? dataInicio, DateTime? dataFim, bool exibirDesistentes)
        {
            return QueryCursistas(cursistaId, cursoId, modalidadeCurso, entidadeId, ufNome, municipioId, dataInicio, dataFim, exibirDesistentes).AsEnumerable().Count();
        }

        public List<GridCursistaDto> PesquisarCursistasSqlQuery(long? cursistaId, long? cursoId, long? modalidadeCurso, long? entidadeId, string ufNome, long? municipioId, DateTime? dataInicio, DateTime? dataFim, bool exibirDesistentes, int start = -1, int count = -1)
        {
            var query = QueryCursistas(cursistaId, cursoId, modalidadeCurso, entidadeId, ufNome, municipioId, dataInicio, dataFim, exibirDesistentes);

            query = query.OrderBy(x => x.Nome);

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

        private IQueryable<GridCursistaDto> QueryCursistas(long? cursistaId, long? cursoId, long? modalidadeCurso, long? entidadeId, string ufNome, long? municipioId, DateTime? dataInicio, DateTime? dataFim, bool exibirDesistentes)
        {
            var cursista = cursistaId.HasValue ? cursistaId.Value : 0;
            var curso = cursoId.HasValue ? cursoId.Value : 0;
            var modalidade = modalidadeCurso.HasValue ? modalidadeCurso.Value : 0;
            var entidade = entidadeId.HasValue ? entidadeId.Value : 0;
            var uf = "%" + ufNome + "%";
            var municipio = municipioId.HasValue ? municipioId.Value : 0;
            var dtInicio = dataInicio.HasValue ? dataInicio.Value.Year.ToString() + '-' + dataInicio.Value.Month.ToString() + '-' + dataInicio.Value.Day.ToString() : "";
            var dtFim = dataFim.HasValue ? dataFim.Value.Year.ToString() + '-' + dataFim.Value.Month.ToString() + '-' + dataFim.Value.Day.ToString() : "";
            var desistentes = Convert.ToInt32(exibirDesistentes);

            var query = _context.GridCursistaDto.FromSqlInterpolated($"GridCursista {cursista}, {curso}, {modalidade}, {entidade}, '{uf}', {municipio}, '{dtInicio}', '{dtFim}', {desistentes}");

            return query;
        }
    }
}