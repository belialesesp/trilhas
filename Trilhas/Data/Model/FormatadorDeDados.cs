namespace Trilhas.Data.Model
{
    public class FormatadorDeDados
    {
        public static string FormatarCPF(string cpf)
        {
            cpf = cpf.Trim();

            if (cpf.Length == 11)
            {
                cpf = cpf.Insert(9, "-");
                cpf = cpf.Insert(6, ".");
                cpf = cpf.Insert(3, ".");
            }

            return cpf;
        }

        public static string FormatarTelefone(string telefone)
        {
            telefone = telefone.Trim();

            if (telefone.Length == 10)
            {
                telefone = telefone.Insert(6, "-");
            }
            else if (telefone.Length == 11)
            {
                telefone = telefone.Insert(7, "-");
            }

            telefone = telefone.Insert(0, "(");
            telefone = telefone.Insert(3, ") ");

            return telefone;
        }

        public static string RemoverFormatacao(string valor)
        {
            if (valor == null)
            {
                return string.Empty;
            }

            return valor.Replace("(", "")
                .Replace(")", "")
                .Replace(" ", "")
                .Replace("-", "")
                .Replace(".", "");
        }
    }
}
