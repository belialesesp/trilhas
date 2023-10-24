using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Data.Model.Certificados;
using Microsoft.EntityFrameworkCore;
using Trilhas.Data.Model.Exceptions;
using Trilhas.Data.Enums;

namespace Trilhas.Services
{
    public class CertificadoEmitidoService
    {
        private ApplicationDbContext _context;

        public CertificadoEmitidoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void GerarCertificadoEmitido(string userId, string codigoAutenticacao, string dados, Pessoa cursista)
        {
            var arquivo = new Arquivo
            {
                Nome = $"certificado-{userId}"
            };

            arquivo.FromString(dados);

            var certificado = new CertificadoEmitido()
            {
                CreationTime = DateTime.Now,
                CreatorUserId = userId,
                Pessoa = cursista,
                CodigoAutenticacao = codigoAutenticacao,
                Hash = dados,
            };

            SalvarCertificadoEmitido(certificado);
        }

        private CertificadoEmitido SalvarCertificadoEmitido(CertificadoEmitido certificado)
        {
            var tx = _context.Database.BeginTransaction();

            try
            {
                _context.CertificadosEmitidos.Add(certificado);

                _context.SaveChanges();

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }

            return certificado;
        }

        /// <summary>
        /// Recupera Cetificado Emitido por Código de Autenticação
        /// </summary>
        /// <returns></returns>
        public CertificadoEmitido RecuperarCertificado(string codigoAutenticacao)
        {
            return _context.CertificadosEmitidos.FirstOrDefault(x => x.CodigoAutenticacao == codigoAutenticacao);
        }

        public List<CertificadoEmitido> RecuperarCertificados(string nome, bool exibirExcluidos)
        {
            IQueryable<CertificadoEmitido> result = _context.CertificadosEmitidos.Include(x => x.Pessoa);

            if (!exibirExcluidos)
            {
                result = result.Where(x => !x.DeletionTime.HasValue);
            }

            if (!string.IsNullOrEmpty(nome))
            {
                result = result.Where(x => x.Pessoa.Nome.ToUpper().Contains(nome.Trim().ToUpper()));
            }

            return result.ToList();
        }

        public int QuantidadeDeCertificados(string nome, bool excluidos)
        {
            return RecuperarCertificados(nome, excluidos).Count;
        }

        public void ExcluirCertificado(string userId, long id)
        {
            CertificadoEmitido certificado = RecuperarCertificado(id);

            if (certificado == null)
            {
                throw new RecordNotFoundException("Registro não encontrado.");
            }

            certificado.DeletionTime = DateTime.Now;
            certificado.DeletionUserId = userId;
            _context.SaveChanges();
        }

        private CertificadoEmitido RecuperarCertificado(long id)
        {
            return _context.CertificadosEmitidos.FirstOrDefault(x => x.Id == id);
        }
    }
}
