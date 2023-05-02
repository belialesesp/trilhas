using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Models.Cadastros.Local;

namespace Trilhas.Controllers.Mappers
{
    public class LocalMapper
    {
        public LocalViewModel MapearLocalViewModel(Local local, List<LocalSala> salasAlocadas)
        {
            LocalViewModel vm = new LocalViewModel(local.Id)
            {
                Nome = local.Nome,
                Observacoes = local.Observacoes,
                Cep = local.Cep,
                Logradouro = local.Logradouro,
                Bairro = local.Bairro,
                Numero = local.Numero,
                Complemento = local.Complemento,
                MunicipioId = local.Municipio.Id,
                Uf = local.Municipio.Uf,
                Salas = MapearLocalSalasViewModel(local.Salas.Where(a => a.DeletionTime == null).ToList(), salasAlocadas),
                Recursos = MapearLocalRecursoViewModel(local.Recursos.Where(a => a.DeletionTime == null).ToList()),
                Contatos = MapearLocalContatoViewModel(local.Contatos.Where(a => a.DeletionTime == null).ToList())
            };

            return vm;
        }

        public List<GridLocalViewModel> MapearLocaisViewModel(List<Local> locais)
        {
            var locaisVm = new List<GridLocalViewModel>();

            foreach (var local in locais)
            {
                var salas = local.Salas.Where(a => !a.DeletionTime.HasValue).ToList();

                locaisVm.Add(new GridLocalViewModel
                {
                    Id = local.Id,
                    Nome = local.Nome,
                    Logradouro = new StringBuilder(local.Logradouro).Append(", ")
                        .Append(local.Numero).Append(", ")
                        .Append(local.Bairro).Append(", ")
                        .Append(local.Municipio.NomeMunicipio.ToUpper()).Append("-")
                        .Append(local.Municipio.Uf).ToString(),
                    QtdSalas = salas.Count,
                    CapacidadeTotal = local.CapacidadeTotal(),
                    Salas = MapearLocalSalaViewModel(salas),
                    Excluido = local.DeletionTime.HasValue
                });
            }

            return locaisVm;
        }

        public List<LocalSalaViewModel> MapearLocalSalasViewModel(List<LocalSala> localSalas, List<LocalSala> salasAlocadas = null)
        {
            List<LocalSalaViewModel> vm = new List<LocalSalaViewModel>();

            foreach (var localSala in localSalas)
            {
                vm.Add(new LocalSalaViewModel(localSala.Id)
                {
                    Sigla = localSala.Sigla,
                    Numero = localSala.Numero,
                    Capacidade = localSala.Capacidade,
                    Alocada = (salasAlocadas != null ? salasAlocadas.Any(x => x.Id == localSala.Id) : false)
                });
            }

            return vm;
        }

        public List<LocalRecursoViewModel> MapearLocalRecursoViewModel(List<LocalRecurso> localRecursos)
        {
            List<LocalRecursoViewModel> vm = new List<LocalRecursoViewModel>();

            foreach (var localRecurso in localRecursos)
            {
                vm.Add(new LocalRecursoViewModel(localRecurso.Id)
                {
                    RecursoId = localRecurso.Recurso.Id,
                    Nome = localRecurso.Recurso.Nome,
                    Descricao = localRecurso.Recurso.Descricao,
                    Quantidade = localRecurso.Quantidade
                });
            }

            return vm;
        }

        public List<LocalContatoViewModel> MapearLocalContatoViewModel(List<LocalContato> localContatos)
        {
            List<LocalContatoViewModel> vm = new List<LocalContatoViewModel>();

            foreach (var localContato in localContatos)
            {
                vm.Add(new LocalContatoViewModel(localContato.Id)
                {
                    TipoContatoId = localContato.TipoContato.Id,
                    Tipo = new TipoContatoViewModel { Nome = localContato.TipoContato.Nome },
                    Numero = localContato.NumeroTelefone
                });
            }

            return vm;
        }

        public List<GridLocalSalaViewModel> MapearLocalSalaViewModel(List<LocalSala> salas)
        {
            List<GridLocalSalaViewModel> salasVM = new List<GridLocalSalaViewModel>();
            foreach (var sala in salas)
            {
                salasVM.Add(new GridLocalSalaViewModel
                {
                    Id = sala.Id,
                    Sigla = sala.Sigla,
                    Capacidade = sala.Capacidade,
                    Numero = sala.Numero
                });
            }
            return salasVM;
        }
    }
}
