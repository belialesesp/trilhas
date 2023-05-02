using System;
using System.Collections.Generic;
using System.Linq;

namespace Trilhas.Data.Model.Trilhas
{
    public class TrilhaDoUsuario : DefaultEntity
    {
        public string UsuarioId { get; set; }

        public List<ItemDaTrilha> Itens { get; set; }

        public List<ItemDaTrilha> ItensAtivos()
        {
            return Itens.Where(x => !x.DeletionTime.HasValue).ToList();
        }

        public TrilhaDoUsuario(string usuarioId)
        {
            this.UsuarioId = usuarioId;
            CreationTime = DateTime.Now;
            CreatorUserId = UsuarioId;
        }

        public bool AdicionarItem(SolucaoEducacional solucao)
        {
            if (Itens == null)
            {
                Itens = new List<ItemDaTrilha>();
            }

            if (!ContemSolucao(solucao))
            {
                Itens.Add(new ItemDaTrilha
                {
                    CreatorUserId = this.UsuarioId,
                    CreationTime = DateTime.Now,
                    SolucaoEducacional = solucao
                });

                LastModificationTime = DateTime.Now;
                LastModifierUserId = this.UsuarioId;

                return true;
            }

            return false;
        }

        public bool RemoverItem(long solucaoId)
        {
            var item = ItensAtivos().FirstOrDefault(x => x.SolucaoEducacional.Id == solucaoId);

            if (item != null)
            {
                item.DeletionTime = DateTime.Now;
                item.DeletionUserId = this.UsuarioId;

                this.LastModificationTime = DateTime.Now;
                this.LastModifierUserId = this.UsuarioId;

                return true;
            }

            return false;
        }

        public bool ContemSolucao(SolucaoEducacional solucao)
        {
            return Itens.Any(x => x.SolucaoEducacional.Id == solucao.Id && !x.DeletionTime.HasValue);
        }

        public bool ContemEstacao(Estacao estacao)
        {
            return Itens.Any(x => x.SolucaoEducacional.Estacao == estacao && !x.DeletionTime.HasValue);
        }
    }
}
