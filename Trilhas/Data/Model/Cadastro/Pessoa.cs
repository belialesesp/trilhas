using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trilhas.Data.Model.Cadastro
{
    public class Pessoa : DefaultEntity
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string NumeroFuncional { get; set; }
        public string NomeSocial { get; set; }
        public DateTime DataNascimento { get; set; }
        public string NumeroIdentidade { get; set; }
        public string Email { get; set; }

        [ForeignKey("OrgaoExpedidorId")]
        public OrgaoExpedidor OrgaoExpedidorIdentidade { get; set; }
        public string UfIdentidade { get; set; }

        public string NumeroTitulo { get; set; }
        public string Pis { get; set; }

        public bool FlagDeficiente { get; set; }
        [ForeignKey("DeficienciaId")]
        public Deficiencia Deficiencia { get; set; }
        [ForeignKey("EscolaridadeId")]
        public Escolaridade Escolaridade { get; set; }

        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        [ForeignKey("MunicipioId")]
        public Municipio Municipio { get; set; }

        [ForeignKey("SexoId")]
        public Sexo Sexo { get; set; }

        public List<PessoaContato> Contatos { get; set; }

        public Entidade Entidade { get; set; }

        [NotMapped]
        public string Imagem { get; set; }
    }
}
