using System;

namespace Trilhas.Models.Trilhas.SolucaoEducacional
{
    public class SalvarVideoViewModel
    {
        public long Id { get; set; }
        public long EstacaoId { get; set; }
        public string Titulo { get; set; }
        public string Responsavel { get; set; }
        public string Url { get; set; }
        public string Duracao { get; set; }
        public DateTime? DataProducao { get; set; }
        public string OutrasInformacoes { get; set; }
    }
}
