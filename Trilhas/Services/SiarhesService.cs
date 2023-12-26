using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Trilhas.Clients;
using Trilhas.Models.Cadastros.Pessoa;
using Trilhas.Services.Interfaces;
using Trilhas.Settings;

namespace Trilhas.Services
{
    public class SiarhesService : SiarhesClient, ISiahresService
    {
        public SiarhesService(HttpClient httpClient, IOptions<SiarhesSettings> settings)
            : base(httpClient, settings)
        {
        }

        public async Task<PessoaSiarhesViewModel> BuscarDadosPessoais(string cpf)
        {
            var response = await HttpClient.GetAsync($"DadosPessoais?cpf={cpf.ToString().PadLeft(11, '0')}&pageNum=1&pageSize=1");
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var pessoa = JsonConvert.DeserializeObject<List<PessoaSiarhesViewModel>>(responseString);

                return pessoa.FirstOrDefault();
            }

            return null;
        }
    }
}
