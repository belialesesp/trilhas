namespace Trilhas.Data.Model
{
    public class Usuario
    {
        public string Apelido { get; set; }
        public bool CpfValidado { get; set; }
        public bool Verificada { get; set; }
        public bool AgentePublico { get; set; }
        public string VerificacaoTipo { get; set; }
        public string SubNovo { get; set; }
        public string Email { get; set; }
        public string Sub { get; set; }
        public string Role { get; set; }
    }
}
