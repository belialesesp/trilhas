namespace Trilhas.Models.Evento.Relatorios
{
    public class RelatorioProgramacaoEventoViewModel
    {
        public string Nome { get; set; }
        public string Sigla { get; set; }
        public string Turma { get; set; }
        public string Modalidade { get; set; }
        public string CargaHoraria { get; set; }
        public string PeriodoRealizacao { get; set; }
        public string PeriodoInscricao { get; set; }
        //public List<string> Horarios { get; set; }
        public string Local { get; set; }
        public int NumeroVagas { get; set; }
        public string Docente { get; set; }
        public string PublicoAlvo { get; set; }
    }
}
