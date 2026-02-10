using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Trilhas.Controllers
{
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public AccountController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task Login(string returnUrl = "/")
        {
            // Use OpenID Connect (Acesso Cidadão)
            await HttpContext.ChallengeAsync("oidc", new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        [Authorize]
        public async Task Logout()
        {
            // Production logout (Acesso Cidadão)
            await HttpContext.SignOutAsync("oidc", new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            });

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}