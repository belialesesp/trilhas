using System.Collections.Generic;

namespace Trilhas.Models.Usuario
{
    public class SolucaoEducacionalOpcaoViewModel
	{
		public long Id { get; set; }
		public long EstacaoId { get; set; }
		public string TipoDeSolucao { get; set; }
		public string Titulo { get; set; }

		// CURSO
		//public string PreRequisitos { get; set; }
		public string PublicoAlvo { get; set; }
		public string ConteudoProgramatico { get; set; }
		public string Modalidade { get; set; }
		public int CargaHorariaTotal { get; set; }
		public List<string> Habilidades { get; set; }

		// LIVRO e VIDEO
		public string Url { get; set; }
		public string Autor { get; set; }
		public string Responsavel { get; set; }
		public string Duracao { get; set; }
		public string Edicao { get; set; }
		public string DataPublicacao { get; set; }
		public string DataProducao { get; set; }

		public SolucaoEducacionalOpcaoViewModel(long id)
		{
			Id = id;
		}
		public bool Selecionado{ get; set; }

	}
}
