using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Trilhas.Models.Cadastros.Pessoa;
using Trilhas.Services.Interfaces;
using Trilhas.Settings;

namespace Trilhas.Services
{
    public class SiarhesService : ISiarhesService
    {
        private readonly HttpClient _httpClient;
        private readonly SiarhesSettings _settings;

        public SiarhesService(HttpClient httpClient, IOptions<SiarhesSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;

            _httpClient.BaseAddress = _settings.BaseUrl;
        }

        public async Task<PessoaSiarhesViewModel> BuscarDadosPessoais(long cpf)
        {
            var response = await _httpClient.GetAsync($"DadosPessoais?cpf={cpf.ToString().PadLeft(11, '0')}&pageNum=1&pageSize=1");
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var pessoa = JsonConvert.DeserializeObject<PessoaSiarhesViewModel>(responseString);

                return pessoa;
            }

            return null;
        }
    }
}
