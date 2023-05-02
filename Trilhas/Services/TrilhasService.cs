using Microsoft.EntityFrameworkCore;
using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Model.Trilhas;

namespace Trilhas.Services
{
    public class TrilhasService
    {
        private ApplicationDbContext _context;

        public TrilhasService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AdicionarTrilha(string usuarioId, SolucaoEducacional solucao)
        {
            var trilha = RecuperarTrilhaDoUsuario(usuarioId);

            if (trilha == null)
            {
                trilha = new TrilhaDoUsuario(usuarioId);
                _context.Trilhas.Add(trilha);
            }

            if (trilha.AdicionarItem(solucao))
            {
                _context.SaveChanges();
            }
        }

        public void RemoverDaTrilha(string usuarioId, long solucaoId)
        {
            var trilha = RecuperarTrilhaDoUsuario(usuarioId);

            if (trilha != null)
            {
                if (trilha.RemoverItem(solucaoId))
                {
                    _context.SaveChanges();
                }
            }
        }

        public TrilhaDoUsuario RecuperarTrilhaDoUsuario(string usuarioId)
        {
            return _context.Trilhas.Include("Itens.SolucaoEducacional")
                .FirstOrDefault(x => x.UsuarioId == usuarioId);
        }

        public TrilhaDoUsuario RecuperarTrilhaCompletaDoUsuario(string usuarioId)
        {
            return _context.Trilhas.Include("Itens.SolucaoEducacional.Estacao.Eixo")
                .FirstOrDefault(x => x.UsuarioId == usuarioId);
        }
    }
}
