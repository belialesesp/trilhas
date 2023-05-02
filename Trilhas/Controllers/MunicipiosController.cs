using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Models.Cadastros.Municipio;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    public class MunicipiosController : DefaultController
    {
        private readonly CadastroService _service;

        public MunicipiosController(UserManager<IdentityUser> userManager, CadastroService service) : base(userManager)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Recuperar(long id)
        {
            Municipio municipio = _service.RecuperarMunicipio(id);
            MunicipioViewModel vm = MapearMunicipioViewModel(municipio);
            return Json(vm);
        }

        [HttpGet]
        public List<string> RecuperarUfs()
        {
            return _service.RecuperarUfs();
        }

        [HttpGet]
        public IActionResult RecuperarMunicipios(string uf)
        {
            if (string.IsNullOrWhiteSpace(uf))
            {
                return Json(new List<Municipio>());
            }

            List<ListaMunicipiosViewModel> vm;

            var municipios = _service.RecuperarMunicipios(uf);
            vm = MapearMunicipiosViewModel(municipios);

            return Json(vm);
        }

        private MunicipioViewModel MapearMunicipioViewModel(Municipio municipio)
        {
            MunicipioViewModel vm = new MunicipioViewModel(municipio.Id)
            {
                NomeMunicipio = municipio.NomeMunicipio,
                codigoMunicipio = municipio.codigoMunicipio,
                codigoUf = municipio.codigoUf,
                Uf = municipio.Uf
            };
            return vm;
        }

        private List<ListaMunicipiosViewModel> MapearMunicipiosViewModel(List<Municipio> municipios)
        {
            List<ListaMunicipiosViewModel> listaRetorno = new List<ListaMunicipiosViewModel>();
            foreach (var municipio in municipios)
            {
                listaRetorno.Add(new ListaMunicipiosViewModel
                {
                    Id = municipio.Id,
                    NomeMunicipio = municipio.NomeMunicipio,
                    Uf = municipio.Uf
                });
            }
            return listaRetorno;
        }

    }
}