using System.Collections.Generic;
using Trilhas.Data.Model.Trilhas;
using Trilhas.Models;
using Trilhas.Models.Trilhas.Estacao;

namespace Trilhas.Controllers.Mappers
{
    public class EstacaoMapper
    {
        public EstacaoViewModel MapearEstacaoViewModel(Estacao estacao)
        {
            EstacaoViewModel estacaoVM = new EstacaoViewModel(estacao.Id);
            estacaoVM.EixoId = estacao.Eixo.Id;
            estacaoVM.Nome = estacao.Nome;
            estacaoVM.Descricao = estacao.Descricao;
            estacaoVM.Excluido = estacao.DeletionTime.HasValue;

            return estacaoVM;
        }

        public List<GridEstacaoViewModel> MapearEstacoesViewModel(List<Estacao> estacoes)
        {
            List<GridEstacaoViewModel> estacoesVm = new List<GridEstacaoViewModel>();

            foreach (var estacao in estacoes)
            {
                estacoesVm.Add(new GridEstacaoViewModel
                {
                    Id = estacao.Id,
                    EixoNome = estacao.Eixo.Nome,
                    Nome = estacao.Nome,
                    Descricao = estacao.Descricao,
                    Excluido = estacao.DeletionTime.HasValue
                });
            }

            return estacoesVm;
        }

        public List<DropDownViewModel> MapearDropDownViewModel(List<Estacao> estacoes)
        {
            var dropdownVm = new List<DropDownViewModel>();

            foreach (var estacao in estacoes)
            {
                dropdownVm.Add(new DropDownViewModel
                {
                    Id = estacao.Id,
                    Nome = estacao.Nome
                });
            }

            return dropdownVm;
        }
    }
}
