using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trilhas.Data.Model;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Models.Cadastros;
using Trilhas.Models.Cadastros.Local;
using Trilhas.Models.Cadastros.Pessoa;
using Trilhas.SqlDto;

namespace Trilhas.Controllers.Mappers
{
    public class PessoaMapper
    {
        public List<OrgaoExpedidorViewModel> MapearOrgaosExpedidoresViewModel(List<OrgaoExpedidor> orgaoExpedidorList)
        {
            List<OrgaoExpedidorViewModel> orgaosExpedidores = new List<OrgaoExpedidorViewModel>();
            foreach (var orgaoExpedidor in orgaoExpedidorList)
            {
                orgaosExpedidores.Add(new OrgaoExpedidorViewModel()
                {
                    Id = orgaoExpedidor.Id,
                    Sigla = orgaoExpedidor.Sigla,
                    Nome = orgaoExpedidor.Nome
                });
            }
            return orgaosExpedidores;
        }

        public List<DeficienciaViewModel> MapearDeficienciasViewModel(List<Deficiencia> orgaoExpedidorList)
        {
            List<DeficienciaViewModel> deficiencias = new List<DeficienciaViewModel>();
            foreach (var deficiencia in orgaoExpedidorList)
            {
                deficiencias.Add(new DeficienciaViewModel()
                {
                    Id = deficiencia.Id,
                    Nome = deficiencia.Nome
                });
            }
            return deficiencias;
        }

        public List<EscolaridadeViewModel> MapearEscolaridadesViewModel(List<Escolaridade> escolaridadeList)
        {
            List<EscolaridadeViewModel> escolaridades = new List<EscolaridadeViewModel>();
            foreach (var escolaridade in escolaridadeList)
            {
                escolaridades.Add(new EscolaridadeViewModel()
                {
                    Id = escolaridade.Id,
                    Nome = escolaridade.Nome
                });
            }
            return escolaridades;
        }

        public PessoaViewModel MapearPessoaViewModel(Pessoa pessoa)
        {
            PessoaViewModel vm = new PessoaViewModel(pessoa.Id)
            {
                Nome = pessoa.Nome,
                NomeSocial = pessoa.NomeSocial,
                Bairro = pessoa.Bairro,
                Cep = pessoa.Cep,
                Complemento = pessoa.Complemento,
                Cpf = pessoa.Cpf,
                DataNascimento = pessoa.DataNascimento,
                FlagDeficiente = pessoa.FlagDeficiente,
                Logradouro = pessoa.Logradouro,
                NumeroFuncional = pessoa.NumeroFuncional,
                NumeroIdentidade = pessoa.NumeroIdentidade,
                Numero = pessoa.Numero,
                NumeroTitulo = pessoa.NumeroTitulo,
                Pis = pessoa.Pis,
                UfIdentidade = pessoa.UfIdentidade,
                Email = pessoa.Email,
                Imagem = pessoa.Imagem,
                EntidadeNome = pessoa.Entidade != null ? pessoa.Entidade.Sigla + " - " + pessoa.Entidade.Nome : string.Empty,
                EntidadeId = pessoa.Entidade?.Id,
                Cidade = pessoa.Municipio?.Id,
                Uf = pessoa.Municipio?.Uf,
                Sexo = pessoa.Sexo?.Id,
                Deficiencia = pessoa.Deficiencia?.Id,
                Escolaridade = pessoa.Escolaridade?.Id,
                OrgaoExpedidorIdentidade = pessoa.OrgaoExpedidorIdentidade?.Id
            };

            if (pessoa.Contatos != null)
            {
                vm.Contatos = MapearPessoaContatoViewModel(pessoa.Contatos.Where(a => a.DeletionTime == null).ToList());
            }

            if (pessoa.Imagem == null)
            {
                vm.StorageError = "Erro ao carregar imagem do serviço de storage.";
            }

            return vm;
        }

        public PessoaViewModel MapearPessoaBuscaPorCpfViewModel(Pessoa pessoa)
        {
            PessoaViewModel vm = new PessoaViewModel(pessoa.Id)
            {
                Nome = pessoa.Nome,
                Cpf = pessoa.Cpf,
                NumeroTitulo = pessoa.NumeroTitulo,
                Pis = pessoa.Pis,
                Email = pessoa.Email
            };

            return vm;
        }

        public List<PessoaContatoViewModel> MapearPessoaContatoViewModel(List<PessoaContato> localContatos)
        {
            List<PessoaContatoViewModel> vm = new List<PessoaContatoViewModel>();

            foreach (var localContato in localContatos)
            {
                vm.Add(new PessoaContatoViewModel(localContato.Id)
                {
                    Tipo = new TipoContatoViewModel { Nome = localContato.TipoContato.Nome },
                    TipoContatoId = localContato.TipoContato.Id,
                    Numero = localContato.Numero
                });
            }
            return vm;
        }

        public List<GridPessoaViewModel> MapearPessoasViewModel(List<Pessoa> pessoas)
        {
            var pessoasVm = new List<GridPessoaViewModel>();

            foreach (var pessoa in pessoas)
            {
                pessoasVm.Add(new GridPessoaViewModel
                {
                    Id = pessoa.Id,
                    Nome = pessoa.Nome,
                    Cpf = FormatadorDeDados.FormatarCPF(pessoa.Cpf),
                    Endereco = new StringBuilder(pessoa.Logradouro).Append(", ")
                        .Append(pessoa.Numero).Append(", ")
                        .Append(pessoa.Bairro).Append(", ")
                        .Append(pessoa.Municipio.NomeMunicipio.ToUpper()).Append("-")
                        .Append(pessoa.Municipio.Uf).ToString(),
                    Email = pessoa.Email,
                    Entidade = pessoa.Entidade != null ? pessoa.Entidade.Sigla : string.Empty,
                    NumeroFuncional = pessoa.NumeroFuncional,
                    Excluido = pessoa.DeletionTime.HasValue
                });
            }

            return pessoasVm;
        }

        public List<GridModalPesquisaViewModel> MapearPessoasModalViewModel(List<Pessoa> pessoas)
        {
            var pessoasVm = new List<GridModalPesquisaViewModel>();

            foreach (var pessoa in pessoas)
            {
                pessoasVm.Add(new GridModalPesquisaViewModel
                {
                    Id = pessoa.Id,
                    Nome = pessoa.NomeSocial ?? pessoa.Nome,
                    NomeSocial = pessoa.NomeSocial,
                    Cpf = FormatadorDeDados.FormatarCPF(pessoa.Cpf),
                    NumeroFuncional = pessoa.NumeroFuncional,
                    Pis = !string.IsNullOrEmpty(pessoa.Pis) ? pessoa.Pis : "",
                    NumeroTitulo = !string.IsNullOrEmpty(pessoa.NumeroTitulo) ? pessoa.NumeroTitulo : ""
                });
            }

            return pessoasVm;
        }

        public List<GridCursistaViewModel> MapearCursistasViewModel(List<GridCursistaDto> cursistas)
        {
            var cursistasVm = new List<GridCursistaViewModel>();

            foreach (var cursista in cursistas)
            {
                cursistasVm.Add(new GridCursistaViewModel
                {
                    Id = cursista.Id,
                    Nome = cursista.Nome,
                    CPF = FormatadorDeDados.FormatarCPF(cursista.CPF),
                    Entidade = cursista.Entidade ?? string.Empty,
                    Municipio = cursista.Municipio ?? string.Empty,
                    Email = cursista.Email ?? string.Empty,
                    //CargaHorariaTotal = cursista.CargaHorariaTotal,
                    QuantidadeEvento = cursista.QuantidadeEvento
                });
            }

            return cursistasVm;
        }
    }
}
