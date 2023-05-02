using System.Collections.Generic;
using Trilhas.Data.Enums;

namespace Trilhas.Models.Trilhas.SolucaoEducacional
{
    public class SolucaoEducacionalViewModel
    {
        public long Id { get; set; }
        public long EstacaoId { get; set; }
        public string TipoDeSolucao { get; set; }
        public string Titulo { get; set; }

        // CURSO
        public string Sigla { get; set; }
        public string Descricao { get; set; }
        public long TipoCursoId { get; set; }
        public EnumModalidade Modalidade { get; set; }
        public long NivelCursoId { get; set; }
        public bool PermiteCertificado { get; set; }
        public int FrequenciaMinimaCertificado { get; set; }
        public int FrequenciaMinimaDeclaracao { get; set; }
        public string PreRequisitos { get; set; }
        public string PublicoAlvo { get; set; }
        public string ConteudoProgramatico { get; set; }
        public int CargaHorariaTotal { get; set; }
        public List<HabilidadeViewModel> Habilidades { get; set; }
        public List<ModuloViewModel> Modulos { get; set; }

        // LIVRO e VIDEO
        public string Url { get; set; }
        public string Autor { get; set; }
        public string Responsavel { get; set; }
        public string Editora { get; set; }
        public string Duracao { get; set; }
        public string Edicao { get; set; }
        public string DataPublicacao { get; set; }
        public string DataProducao { get; set; }
        public string OutrasInformacoes { get; set; }

        public SolucaoEducacionalViewModel(long id)
        {
            Id = id;
        }
    }
}
