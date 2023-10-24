using Newtonsoft.Json;
using System;

namespace Trilhas.Models.Cadastros.Pessoa
{
    public class PessoaSiarhesViewModel
    {
        [JsonProperty("numfunc")]
        public int NumeroFuncional { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("deficiente")]
        public string Deficiente { get; set; }

        [JsonProperty("dataNascimento")]
        public DateTime DataNascimento { get; set; }

        [JsonProperty("nomeLogradouro")]
        public string Logradouro { get; set; }

        [JsonProperty("numeroLogradouro")]
        public int Numero { get; set; }

        [JsonProperty("complementoLogradouro")]
        public string Complemento { get; set; }

        [JsonProperty("bairroLogradouro")]
        public string Bairro { get; set; }

        [JsonProperty("ufLogradouro")]
        public string Uf { get; set; }

        [JsonProperty("cepLogradouro")]
        public long Cep { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
