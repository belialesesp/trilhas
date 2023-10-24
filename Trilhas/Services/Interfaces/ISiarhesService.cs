using System.Threading.Tasks;
using Trilhas.Models.Cadastros.Pessoa;

namespace Trilhas.Services.Interfaces
{
    public interface ISiarhesService
    {
        Task<PessoaSiarhesViewModel> BuscarDadosPessoais(long cpf);
    }
}
