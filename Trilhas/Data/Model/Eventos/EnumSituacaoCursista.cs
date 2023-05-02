using System.ComponentModel;

namespace Trilhas.Data.Model.Eventos
{
    public enum EnumSituacaoCursista
    {
        [Description("CERTIFICADO")]
        CURSANDO = 0,

        [Description("CERTIFICADO")]
        CERTIFICADO = 1,

        [Description("DECLARADO")]
        DECLARADO = 2,

        [Description("DESISTENTE")]
        DESISTENTE = 3
    }
}
