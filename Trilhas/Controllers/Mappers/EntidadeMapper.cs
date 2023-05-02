using System.Collections.Generic;
using Trilhas.Data.Model;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Models.Cadastros;
using Trilhas.Models.Cadastros.Pessoa;
using Trilhas.Models.Cadastros.TipoEntidade;

namespace Trilhas.Controllers.Mappers
{
    public class EntidadeMapper
    {
        public EntidadeViewModel MapearEntidadeViewModel(Entidade entidade)
        {
            EntidadeViewModel vm = new EntidadeViewModel(entidade.Id);
            vm.Sigla = entidade.Sigla;
            vm.Nome = entidade.Nome;
            vm.MunicipioId = entidade.Municipio.Id;
            vm.Uf = entidade.Municipio.Uf;
            vm.TipoEntidadeId = entidade.Tipo.Id;

            vm.Gestores = new List<GridModalPesquisaViewModel>();

            if (entidade.Gestores != null)
            {
                foreach (var gestor in entidade.Gestores)
                {
                    vm.Gestores.Add(new GridModalPesquisaViewModel
                    {
                        Id = gestor.Pessoa.Id,
                        Nome = gestor.Pessoa.NomeSocial ?? gestor.Pessoa.Nome,
                        NomeSocial = gestor.Pessoa.NomeSocial,
                        Cpf = FormatadorDeDados.FormatarCPF(gestor.Pessoa.Cpf),
                        NumeroFuncional = gestor.Pessoa.NumeroFuncional
                    });
                }
            }

            return vm;
        }

        public EntidadeViewModel MapearEntidadeBasicaViewModel(Entidade entidade)
        {
            EntidadeViewModel vm = new EntidadeViewModel(entidade.Id) {
                Sigla = entidade.Sigla,
                Nome = entidade.Nome,
                TipoEntidadeId = entidade.TipoId,
            };

            return vm;
        }

        public List<GridEntidadeViewModel> MapearEntidadesViewModel(List<Entidade> entidades)
        {
            var entidadesVm = new List<GridEntidadeViewModel>();

            foreach (var entidade in entidades)
            {
                entidadesVm.Add(new GridEntidadeViewModel
                {
                    Id = entidade.Id,
                    Nome = entidade.Sigla + " - " + entidade.Nome,
                    Municipio = entidade.Municipio.NomeMunicipio + "-" + entidade.Municipio.Uf,
                    Tipo = entidade.Tipo.Descricao,
                    Excluido = entidade.DeletionTime.HasValue
                });
            }

            return entidadesVm;
        }

        public List<TipoDeEntidadeViewModel> MapearTipoEntidadeViewModel(List<TipoDeEntidade> tipos)
        {
            List<TipoDeEntidadeViewModel> listaRetorno = new List<TipoDeEntidadeViewModel>();

            if (tipos != null)
            {
                foreach (var tipo in tipos)
                {
                    listaRetorno.Add(new TipoDeEntidadeViewModel(tipo.Id)
                    {
                        Descricao = tipo.Descricao
                    });
                }
            }

            return listaRetorno;
        }
    }
}
