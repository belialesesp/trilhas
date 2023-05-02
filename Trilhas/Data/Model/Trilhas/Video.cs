using System;

namespace Trilhas.Data.Model.Trilhas
{
    public class Video : SolucaoEducacional
    {
        public string Responsavel { get; set; }
        public DateTime? DataProducao { get; set; }
        public string Url { get; set; }
        public string Duracao { get; set; }
        public string OutrasInformacoes { get; set; }
    }
}
