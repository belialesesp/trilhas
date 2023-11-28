using Trilhas.Models.Cadastros.Local;

namespace Trilhas.Models.Cadastros.Pessoa
{
    public class PessoaContatoViewModel
	{
		public long? Id { get; set; }
		public string Numero { get; set; }
        public long TipoContatoId { get; set; }
        public TipoContatoViewModel TipoContato { get; set; }

        public PessoaContatoViewModel()
        {
                
        }
        public PessoaContatoViewModel(long id){
			Id = id;
		}
	}
}
