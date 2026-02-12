using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Trilhas.Data.Model.Cadastro;
using Trilhas.Models.Evento;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    [Authorize(Roles = "Administrador,Secretaria")]
    public class EtiquetaController : Controller
    {
        protected UserManager<IdentityUser> _userManager;
        private readonly PessoaService _pessoaService;

        public EtiquetaController(UserManager<IdentityUser> userManager, PessoaService pessoaService)
        {
            _userManager = userManager;
            _pessoaService = pessoaService;
        }

        public IActionResult Index(long cursistaId, long eventoId)
        {
            Pessoa cursista = _pessoaService.RecuperarPessoa(cursistaId);
            EtiquetaViewModel vm = new EtiquetaViewModel();
            vm.Nome = cursista.NomeSocial ?? cursista.Nome;
            vm.Nome = vm.Nome.Split(" ")[0];
            if (vm.Nome.Length > 15)
            {
                vm.Nome = vm.Nome.Substring(0, 14);
            }
            vm.Entidade = cursista.Entidade.Sigla;
            vm.CursistaId = cursistaId;
            vm.EventoId = eventoId;

            return View(vm);
        }
    }
}