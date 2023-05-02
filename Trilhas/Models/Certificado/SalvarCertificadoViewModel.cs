using Trilhas.Data.Enums;

namespace Trilhas.Models.Certificado
{
    public class SalvarCertificadoViewModel
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Dados { get; set; }
        public bool Padrao { get; set; }
        public EnumTipoCertificado TipoCertificado { get; set; }
    }
}
