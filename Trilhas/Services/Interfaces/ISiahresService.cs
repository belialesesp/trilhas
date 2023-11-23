using System.Threading.Tasks;
using Trilhas.Models.Cadastros.Pessoa;

namespace Trilhas.Services.Interfaces
{
    public interface ISiahresService
    {
        Task<PessoaSiarhesViewModel> BuscarDadosPessoais(string cpf);
    }
}
