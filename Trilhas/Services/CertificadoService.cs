using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Enums;
using Trilhas.Data.Model.Certificados;
using Trilhas.Data.Model.Exceptions;

namespace Trilhas.Services
{
    public class CertificadoService
    {
        private ApplicationDbContext _context;

        public CertificadoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Certificado SalvarCertificado(string userId, Certificado certificado)
        {
            var tx = _context.Database.BeginTransaction();

            try
            {
                if (certificado.Id > 0)
                {
                    ValidarPossibilidadeDeEdicao(certificado);

                    certificado.LastModifierUserId = userId;
                    certificado.LastModificationTime = DateTime.Now;
                }
                else
                {
                    certificado.CreatorUserId = userId;
                    certificado.CreationTime = DateTime.Now;

                    _context.Certificados.Add(certificado);
                }

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
        
        private void ValidarPossibilidadeDeEdicao(Certificado certificado)
        {
            var possuiEventos = _context.Eventos
                .Include(x => x.Certificado)
                .Include(x => x.ListaDeInscricao).ThenInclude(x => x.Inscritos)
                .Any(x => x.Certificado.Id == certificado.Id && !x.DeletionTime.HasValue && x.ListaDeInscricao.Inscritos.Any(y => y.Situacao != EnumSituacaoCursista.DESISTENTE && !y.DeletionTime.HasValue));

            if (possuiEventos)
            {
                throw new TrilhasException("Este Certificado já foi emitido para alguns Cursistas e não pode mais ser alterado.");
            }
        }

        public void ExcluirCertificado(string userId, long id)
        {
            Certificado certificado = RecuperarCertificado(id, null);

            if (certificado == null)
            {
                throw new RecordNotFoundException("Registro não encontrado.");
            }

            certificado.DeletionTime = DateTime.Now;
            certificado.DeletionUserId = userId;
            _context.SaveChanges();
        }

        public Certificado RecuperarCertificado(long id, EnumTipoCertificado? tipoCertificado)
        {
            return _context.Certificados.FirstOrDefault(x => x.Id == id && (!tipoCertificado.HasValue || x.TipoCertificado == tipoCertificado));
        }

        public Certificado RecuperarCertificadoPadrao(EnumTipoCertificado tipoCertificado)
        {
            return _context.Certificados.FirstOrDefault(x => x.Padrao && x.TipoCertificado == tipoCertificado);
        }

        public Certificado RecuperarCertificadoPorTipo(EnumTipoCertificado tipoCertificado)
        {
            return _context.Certificados.FirstOrDefault(x => x.TipoCertificado == tipoCertificado);
        }

        public List<Certificado> RecuperarCertificados(string nome, bool exibirExcluidos, EnumTipoCertificado? tipoCertificado, int start = -1, int count = -1)
        {
            IQueryable<Certificado> result = _context.Certificados;

            if (!exibirExcluidos)
            {
                result = result.Where(x => !x.DeletionTime.HasValue);
            }

            if (!string.IsNullOrEmpty(nome))
            {
                result = result.Where(x => x.Nome.ToUpper().Contains(nome.Trim().ToUpper()));
            }
            if (tipoCertificado.HasValue)
            {
                result = result.Where(x => x.TipoCertificado == tipoCertificado.Value);
            }

            return result.ToList();
        }

        public int QuantidadeDeCertificados(string nome, bool excluidos, EnumTipoCertificado? tipoCertificado, int start = -1, int count = -1)
        {
            return RecuperarCertificados(nome, excluidos, tipoCertificado, start, count).Count;
        }

		public Certificado RecuperarDeclaracao(){
			return _context.Certificados
				.FirstOrDefault(x => x.TipoCertificado == EnumTipoCertificado.DeclaracaoCursita && !x.DeletionTime.HasValue);
		}

        public string CodigoAutenticacaoEletronica()
        {
            int numero = new Random().Next(1000, 9999);
            int hora = DateTime.Now.Hour;
            int minuto = DateTime.Now.Minute;
            int segundo = DateTime.Now.Second;
            int data = Convert.ToInt32(DateTime.Now.ToString("yyMMdd"));

            string aux = data.ToString("X").PadLeft(5, '0');

            hora = hora * 3600 + minuto * 60 + segundo;

            string aux2 = hora.ToString("X").PadLeft(5, '0');

            string aux3 = numero.ToString("X").PadLeft(4, '0');

            return aux3 + aux + aux2;
        }
    }
}
