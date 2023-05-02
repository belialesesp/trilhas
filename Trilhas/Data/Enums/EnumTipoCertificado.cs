using System.ComponentModel;

namespace Trilhas.Data.Enums
{
    public enum EnumTipoCertificado
    {
        [Description("CERTIFICADO")]
        Certificado,
        [Description("DECLARAÇÃO DE CURSISTA")]
        DeclaracaoCursita,
        [Description("DECLARAÇÃO DE DOCENTE")]
        DeclaracaoDocente
    }
}
