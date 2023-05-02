using System.Collections.Generic;
using Trilhas.Models.Cadastros.Pessoa;

namespace Trilhas.Models.Cadastros
{
    public class EntidadeViewModel
    {
        public long Id { get; set; }
        public string Sigla { get; set; }
        public string Nome { get; set; }
        public long TipoEntidadeId { get; set; }
        public string Uf { get; set; }
        public long MunicipioId { get; set; }
        public List<GridModalPesquisaViewModel> Gestores { get; set; }

        public EntidadeViewModel(long id)
        {
            Id = id;
        }
        //public List<Pessoa> Gestor { get; set; }
    }
}
