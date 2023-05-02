using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Model.Trilhas;

namespace Trilhas.Services
{
    public class EixoService
    {
        private ApplicationDbContext _context;
        private readonly EstacaoService _estacaoService;
        private MinioService _minioService;

        public EixoService(ApplicationDbContext context,
        EstacaoService estacaoService,
            MinioService minioService)
        {
            _context = context;
            _minioService = minioService;
            _estacaoService = estacaoService;
        }

        public Eixo SalvarEixo(string userId, Eixo eixo)
        {
            ValidarDuplicidade(eixo);

            var tx = _context.Database.BeginTransaction();

            try
            {
                if (eixo.Id > 0)
                {
                    eixo.LastModifierUserId = userId;
                    eixo.LastModificationTime = DateTime.Now;
                }
                else
                {
                    eixo.CreatorUserId = userId;
                    eixo.CreationTime = DateTime.Now;

                    _context.Eixos.Add(eixo);
                }

                _context.SaveChanges();

                if (eixo.Imagem == null)
                {
                    _minioService.ExcluirImagemEixo("eixo-" + eixo.Id);
                }
                else
                {
                    Arquivo imagem = new Arquivo
                    {
                        Nome = "eixo-" + eixo.Id
                    };
                    imagem.FromBase64(eixo.Imagem);

                    _minioService.SalvarImagemEixo(imagem);
                }

                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }

            return eixo;
        }

        private void ValidarDuplicidade(Eixo eixo)
        {
            if (_context.Eixos.Any(x => x.Nome == eixo.Nome && x.Id != eixo.Id && !x.DeletionTime.HasValue))
            {
                throw new TrilhasValidationException("Já existe um Eixo cadastrado com o mesmo nome.");
            }
        }

        public void ExcluirEixo(string userId, long id)
        {
            var eixo = RecuperarEixo(id);

            if (eixo == null)
            {
                throw new RecordNotFoundException("Registro não encontrado.");
            }

            if (!PodeExcluirEixo(eixo))
            {
                throw new ConstraintException("Existem Estações vinculadas ao Eixo.");
            }

            eixo.DeletionTime = DateTime.Now;
            eixo.DeletionUserId = userId;

            _minioService.ExcluirImagemEixo("eixo-" + id);

            _context.SaveChanges();
        }

        private bool PodeExcluirEixo(Eixo eixo)
        {
            return _estacaoService.RecupararEstacoesEixo(eixo.Id).Count == 0;
        }

        public Eixo RecuperarEixo(long id, bool incluirImagem = false, bool incluirExcluidos = false)
        {
            var eixo = _context.Eixos.FirstOrDefault(x => x.Id == id && (!x.DeletionTime.HasValue || incluirExcluidos));

            if (eixo != null && incluirImagem)
            {
                eixo.Imagem = RecuperarImagemEixo(eixo.Id);
            }

            return eixo;
        }

        public string RecuperarImagemEixo(long eixoId)
        {
            Arquivo arquivo = _minioService.RecuperarImagemEixo("eixo-" + eixoId).Result;

            if (arquivo != null)
            {
                return arquivo.ArquivoBase64;
            }

            return null;
        }

        public List<Eixo> RecupararEixosComImagem()
        {
            List<Eixo> eixos = _context.Eixos
                .Where(x => (!x.DeletionTime.HasValue))
                .ToList();

            Arquivo imagem;
            foreach (var eixo in eixos)
            {
                imagem = _minioService.RecuperarImagemEixo("eixo-" + eixo.Id).Result;
                if (imagem != null)
                {
                    eixo.Imagem = imagem.ArquivoBase64;
                }
            }
            return eixos;
        }

        public int QuantidadeDeEixos(string nome, bool excluidos)
        {
            return PesquisarEixo(nome, excluidos).Count();
        }

        public List<Eixo> PesquisarEixos(string nome, bool excluidos, int start = -1, int count = -1)
        {
            return PesquisarEixo(nome, excluidos, start, count).ToList();
        }

        private IQueryable<Eixo> PesquisarEixo(string nome, bool exibirExcluidos, int start = -1, int count = -1)
        {
            IQueryable<Eixo> result = _context.Eixos;

            if (!exibirExcluidos)
            {
                result = result.Where(x => !x.DeletionTime.HasValue);
            }
            if (!string.IsNullOrEmpty(nome))
            {
                result = result.Where(x => x.Nome.ToUpper().Contains(nome.Trim().ToUpper()));
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
