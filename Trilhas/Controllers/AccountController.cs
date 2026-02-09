using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Security.Claims;
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
            // Check if we're in development mode
            if (_env.IsDevelopment() || Environment.GetEnvironmentVariable("BYPASS_AUTH") == "true")
            {
                // Redirect to local login page
                Response.Redirect($"/Account/LocalLogin?returnUrl={Uri.EscapeDataString(returnUrl)}");
                return;
            }

            // Production: Use OpenID Connect (Acesso Cidadão)
            await HttpContext.ChallengeAsync("oidc", new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        [HttpGet]
        public IActionResult LocalLogin(string returnUrl = "/")
        {
            // Only allow local login in development
            if (!_env.IsDevelopment() && Environment.GetEnvironmentVariable("BYPASS_AUTH") != "true")
            {
                return RedirectToAction("Login", new { returnUrl });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LocalLogin(string username, string role, string returnUrl = "/")
        {
            // Only allow local login in development
            if (!_env.IsDevelopment() && Environment.GetEnvironmentVariable("BYPASS_AUTH") != "true")
            {
                return RedirectToAction("Login", new { returnUrl });
            }

            // Create test claims for local development
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username ?? "TestUser"),
                new Claim("name", username ?? "Test User"),
                new Claim("email", $"{username ?? "testuser"}@localhost.local"),
                new Claim("subnovo", Guid.NewGuid().ToString()),
                new Claim("verificada", "true")
            };

            // Add role based on selection
            switch (role)
            {
                case "admin":
                    claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
                    break;
                case "secretaria":
                    claims.Add(new Claim(ClaimTypes.Role, "Secretaria"));
                    break;
                case "gestor":
                    claims.Add(new Claim(ClaimTypes.Role, "Gestor"));
                    break;
                case "coordenador":
                    claims.Add(new Claim(ClaimTypes.Role, "Coordenador"));
                    break;
                case "all":
                    // Add all roles for complete testing
                    claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
                    claims.Add(new Claim(ClaimTypes.Role, "Secretaria"));
                    claims.Add(new Claim(ClaimTypes.Role, "Gestor"));
                    claims.Add(new Claim(ClaimTypes.Role, "Coordenador"));
                    break;
                default:
                    // Default user with no special roles
                    break;
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = returnUrl
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        public async Task Logout()
        {
            if (_env.IsDevelopment() || Environment.GetEnvironmentVariable("BYPASS_AUTH") == "true")
            {
                // Local logout
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                Response.Redirect("/");
                return;
            }

            // Production logout (Acesso Cidadão)
            await HttpContext.SignOutAsync("oidc", new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            });

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}