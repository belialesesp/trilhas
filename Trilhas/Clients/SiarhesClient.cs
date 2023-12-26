using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Trilhas.Extensions;
using Trilhas.Settings;

namespace Trilhas.Clients
{
    public class SiarhesClient
    {
        private const string GRANT_TYPE = "client_credentials";
        private const string URL_TOKEN = "connect/token";
        private readonly HttpClient _httpClient;
        private readonly SiarhesSettings _settings;

        public SiarhesClient(HttpClient httpClient, IOptions<SiarhesSettings> settings)
        {
            _settings = settings.Value;
            var base64EncodedString = $"{_settings.ClientId}:{_settings.ClientSecret}".EncodeBase64();

            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", GRANT_TYPE),
                new KeyValuePair<string, string>("scope", _settings.Scope)
            };

            var data = new FormUrlEncodedContent(nvc);

            _httpClient = httpClient;
            _httpClient.BaseAddress = _settings.BaseUrl;
            
            var httpPost = new HttpClient();
            httpPost.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedString);
            var postResponse = httpPost.PostAsync(_settings.Authority + URL_TOKEN, data).Result;

            string result = postResponse.Content.ReadAsStringAsync().Result;

            dynamic parseado = JObject.Parse(result);

            _httpClient.SetBearerToken((string)parseado.access_token);
        }

        public HttpClient HttpClient { get { return _httpClient; } }    
    }
}
