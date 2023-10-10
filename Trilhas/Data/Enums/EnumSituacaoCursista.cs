using System.ComponentModel;

namespace Trilhas.Data.Enums
{
    public enum EnumSituacaoCursista
    {
        [Description("CURSANDO")]
        CURSANDO = 0,

        [Description("CERTIFICADO")]
        CERTIFICADO = 1,

        [Description("DECLARADO")]
        DECLARADO = 2,

        [Description("DESISTENTE")]
        DESISTENTE = 3
    }
}
