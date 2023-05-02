namespace Trilhas.SqlDto
{
    public class GridDocenteDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public int Excluido { get; set; }
        public int QuantidadeEvento { get; set; }
        public int? CargaHorariaTotal { get; set; }
    }
}
