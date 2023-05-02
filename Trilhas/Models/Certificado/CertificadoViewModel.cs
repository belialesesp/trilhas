using Trilhas.Data.Enums;

namespace Trilhas.Models.Certificado
{
    public class CertificadoViewModel
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Dados { get; set; }
        public bool Padrao { get; set; }
        public bool Excluido { get; set; }
        public EnumTipoCertificado TipoCertificado { get; set; }
        public string TipoCertificadoDescricao
        {
            get
            {
                return Extensions.EnumDescription.GetDescription((EnumTipoCertificado)TipoCertificado);
            }
            set { }
        }

        public CertificadoViewModel(long id)
        {
            this.Id = id;
        }
    }
}
