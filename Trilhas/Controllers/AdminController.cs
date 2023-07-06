using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Trilhas.Models;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    [Authorize(Roles = "Administrador,Secretaria,Gestor,Coordenador")]
    public class AdminController : Controller
    {
        private MinioService _minioService;
        protected UserManager<IdentityUser> _userManager;

        public AdminController(MinioService minioService, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _minioService = minioService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StorageStatus()
        {
            var obj = new StorageError
            {
                Erro = _minioService.UltimoErro(),
                Hora = _minioService.HoraUltimoErro().ToString("dd/MM/yyyy HH:mm:ss")
            };

            return Json(obj);
        }

        public IActionResult StorageLastError()
        {
            return Content(_minioService.UltimoErro());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("/health/live")]
        [AllowAnonymous]
        public IActionResult Ping()
        {
            return Json("healthy");
        }
    }

    class StorageError
    {
        public string Erro { get; set; }
        public string Hora { get; set; }
    }


    public class AdminViewModel
    {
        public ClaimsPrincipal claims { get; set; }
    }
}
