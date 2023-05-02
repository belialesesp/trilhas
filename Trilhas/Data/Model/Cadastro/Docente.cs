using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Trilhas.Data.Model.Trilhas;

namespace Trilhas.Data.Model.Cadastro
{
    public class Docente : DefaultEntity
    {
        [ForeignKey("PessoaId")]
        public Pessoa Pessoa { get; set; }
        public string Observacoes { get; set; }
        public List<DadosBancarios> DadosBancarios { get; set; }
        public List<Habilitacao> Habilitacao { get; set; }
        public List<Formacao> Formacao { get; set; }

        public void AdicionarHabilitacao(Curso curso)
        {
            if (Habilitacao == null)
            {
                Habilitacao = new List<Habilitacao>();
            }
            else if (Habilitacao.Any(x => x.Curso.Id == curso.Id))
            {
                return;
            }

            Habilitacao.Add(new Cadastro.Habilitacao
            {
                Curso = curso,
                Docente = this
            });
        }
    }
}
