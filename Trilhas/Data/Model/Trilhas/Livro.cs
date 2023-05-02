using System;

namespace Trilhas.Data.Model.Trilhas
{
    public class Livro : SolucaoEducacional
    {
        public string Autor { get; set; }
        public DateTime? DataPublicacao { get; set; }
        public string Url { get; set; }
        public string Editora { get; set; }
        public string Edicao { get; set; }
        public string OutrasInformacoes { get; set; }
    }
}
