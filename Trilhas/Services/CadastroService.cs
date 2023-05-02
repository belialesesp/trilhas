using System.Collections.Generic;
using System.Linq;
using Trilhas.Data;
using Trilhas.Data.Model.Cadastro;

namespace Trilhas.Services
{
    public class CadastroService
    {
        private ApplicationDbContext _context;

        public CadastroService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Municipio RecuperarMunicipio(long codigoMunicipio)
        {
            return _context.Municipios.FirstOrDefault(x => x.Id == codigoMunicipio);
        }

        //public long RecuperarIdMunicipio(string municipioNome)
        //{
        //    return _context.Municipios.FirstOrDefault(x => x.NomeMunicipio == municipioNome).Id;
        //}

        public List<Municipio> RecuperarMunicipios(string uf)
        {
            return _context.Municipios
                .Where(x => x.Uf == uf)
                .OrderBy(x => x.NomeMunicipio)
                .ToList();
        }

        public List<string> RecuperarUfs()
        {
            return _context.Municipios.OrderBy(x => x.Uf).Select(x => x.Uf).Distinct().ToList();
        }
    }
}
