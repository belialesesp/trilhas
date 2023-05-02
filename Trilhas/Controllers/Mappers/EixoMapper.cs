using System.Collections.Generic;
using Trilhas.Data.Model.Trilhas;
using Trilhas.Models;
using Trilhas.Models.Trilhas.Eixo;

namespace Trilhas.Controllers.Mappers
{
    public class EixoMapper
    {
        public EixoViewModel MapearEixoViewModel(Eixo eixo)
        {
            var vm = new EixoViewModel(eixo.Id);
            vm.Nome = eixo.Nome;
            vm.Descricao = eixo.Descricao;
            vm.Imagem = eixo.Imagem;
            vm.Excluido = eixo.DeletionTime.HasValue;

            if (eixo.Imagem == null)
            {
                vm.StorageError = "Erro ao carregar imagem do serviço de storage.";
            }

            return vm;
        }

        public List<GridEixoViewModel> MapearEixosViewModel(List<Eixo> eixos)
        {
            var eixosVm = new List<GridEixoViewModel>();

            foreach (var eixo in eixos)
            {
                eixosVm.Add(new GridEixoViewModel
                {
                    Id = eixo.Id,
                    Nome = eixo.Nome,
                    Descricao = eixo.Descricao,
                    Excluido = eixo.DeletionTime.HasValue
                });
            }

            return eixosVm;
        }

        public List<DropDownViewModel> MapearDropDownViewModel(List<Eixo> eixos)
        {
            var dropdownVm = new List<DropDownViewModel>();

            foreach (var eixo in eixos)
            {
                dropdownVm.Add(new DropDownViewModel
                {
                    Id = eixo.Id,
                    Nome = eixo.Nome
                });
            }

            return dropdownVm;
        }
    }
}
