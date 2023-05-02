using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Model.Eventos;

namespace Trilhas.Services
{
    public class ListaPresencaService
    {
        private ApplicationDbContext _context;

        public ListaPresencaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public RegistroDePresenca SalvarListaPresenca(RegistroDePresenca listaPresenca)
        {
            if (listaPresenca.Id == 0)
            {
                _context.RegistrosDePresenca.Add(listaPresenca);
            }
            else
            {
                _context.RegistrosDePresenca.Update(listaPresenca);
            }

            _context.SaveChanges();
            return listaPresenca;
        }

        public RegistroDePresenca RecuperarListaPresenca(long eventoHorarioId, long pessoaId)
        {
            return _context.RegistrosDePresenca.FirstOrDefault(x => x.EventoHorario.Id == eventoHorarioId && x.Pessoa.Id == pessoaId);
        }
    }
}
