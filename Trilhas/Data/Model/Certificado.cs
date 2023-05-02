using Trilhas.Data.Enums;

namespace Trilhas.Data.Model
{
    public class Certificado : DefaultEntity
    {
        public string Nome { get; set; }
        public string Dados { get; set; }
        public bool Padrao { get; set; }
        public EnumTipoCertificado TipoCertificado { get; set; }
    }
}
