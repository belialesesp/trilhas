namespace Trilhas.Models.CertificadoEmitido
{
    public class CertificadoEmitidoViewModel
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public bool Excluido { get; set; }

        public CertificadoEmitidoViewModel(long id)
        {
            Id = id;
        }
    }   
}
