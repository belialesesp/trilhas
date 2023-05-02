using System.Collections.Generic;

namespace Trilhas.Models.Cadastros.Docente
{
    public class SalvarDocenteViewModel
    {
        public long Id { get; set; }
        public long PessoaId { get; set; }
        public List<DocenteDadosBancariosViewModel> DadosBancarios { get; set; }
        public List<DocenteHabilitacaoViewModel> Habilitacao { get; set; }
        public List<DocenteFormacaoViewModel> Formacao { get; set; }
        public string Observacoes { get; set; }
        public string Pis { get; set; }
        public string Titulo { get; set; }
    }
}
