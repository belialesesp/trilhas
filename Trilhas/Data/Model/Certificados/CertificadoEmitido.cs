using System.ComponentModel.DataAnnotations.Schema;
using Trilhas.Data.Model.Cadastro;

namespace Trilhas.Data.Model.Certificados
{
    public class CertificadoEmitido : NotModifierEntity
    {
        [ForeignKey("PessoaId")]
        public Pessoa Pessoa { get; set; }

        public string Hash { get; set; }
        public string CodigoAutenticacao { get; set; }
    }
}
