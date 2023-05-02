namespace Trilhas.Models.Cadastros.Pessoa
{
    public class GridCursistaViewModel
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Entidade { get; set; }
        public string Municipio { get; set; }
        public string Email { get; set; }
        public int QuantidadeEvento { get; set; }
        public int? CargaHorariaTotal { get; set; }
    }
}
