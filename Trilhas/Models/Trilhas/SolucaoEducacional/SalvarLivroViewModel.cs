using System;

namespace Trilhas.Models.Trilhas.SolucaoEducacional
{
    public class SalvarLivroViewModel
    {
        public long Id { get; set; }
        public long EstacaoId { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Url { get; set; }
        public string Editora { get; set; }
        public DateTime? DataPublicacao { get; set; }
        public string Edicao { get; set; }
        public string OutrasInformacoes { get; set; }
    }
}
