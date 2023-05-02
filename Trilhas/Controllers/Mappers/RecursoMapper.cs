using System.Collections.Generic;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Models.Cadastros.Recurso;

namespace Trilhas.Controllers.Mappers
{
    public class RecursoMapper
    {
        public RecursoViewModel MapearRecursoViewModel(Recurso recurso)
        {
            RecursoViewModel vm = new RecursoViewModel(recurso.Id);
            vm.Nome = recurso.Nome;
            vm.Descricao = recurso.Descricao;
            vm.Custo = recurso.Custo;
            return vm;
        }

        public List<GridRecursoViewModel> MapearRecursosViewModel(List<Recurso> recursos)
        {
            var recursosVm = new List<GridRecursoViewModel>();

            foreach (var recurso in recursos)
            {
                recursosVm.Add(new GridRecursoViewModel
                {
                    Id = recurso.Id,
                    Nome = recurso.Nome,
                    Descricao = recurso.Descricao,
                    Custo = recurso.Custo,
                    Excluido = recurso.DeletionTime.HasValue
                });
            }

            return recursosVm;
        }
    }
}
