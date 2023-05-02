namespace Trilhas.Models.Trilhas.Eixo
{
    public class EixoViewModel
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Imagem { get; set; }
        public bool Excluido { get; set; }
        public string StorageError { get; set; }

        public EixoViewModel(long id)
        {
            Id = id;
        }
    }
}
