using System.Collections.Generic;
using Trilhas.Data.Model.Certificados;
using Trilhas.Models.CertificadoEmitido;

namespace Trilhas.Controllers.Mappers
{
    public class CertificadoEmitidoMapper
    {
        public List<CertificadoEmitidoViewModel> MapearGridCertificadoEmitido(List<CertificadoEmitido> certificados)
        {
            List<CertificadoEmitidoViewModel> certificadoVm = new List<CertificadoEmitidoViewModel>();

            foreach (var certificado in certificados)
            {
                certificadoVm.Add(new CertificadoEmitidoViewModel(certificado.Id)
                {
                    Nome = certificado.Pessoa.Nome,
                    Excluido = certificado.DeletionTime.HasValue
                });
            }

            return certificadoVm;
        }
    }
}
