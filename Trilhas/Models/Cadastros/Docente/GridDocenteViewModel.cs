namespace Trilhas.Models.Cadastros.Docente
{
    public class GridDocenteViewModel
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public bool Excluido { get; set; }
        public decimal? Avaliacao { get; set; }
        public decimal? QuantidadeEnvento { get; set; }
        public double? CargaHorariaTotal { get; set; }
    }
}
