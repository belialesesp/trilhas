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

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            // Check if we're in development mode
            if (_env.IsDevelopment() || Environment.GetEnvironmentVariable("BYPASS_AUTH") == "true")
            {
                // Redirect to local login page
                return RedirectToAction("LocalLogin", new { returnUrl });
            }

            // Production: Use OpenID Connect (Acesso Cidadão)
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl }, "oidc");
        }

        /// <summary>
        /// GET: Display the local login form (MISSING ACTION - THIS WAS THE BUG!)
        /// </summary>
        [HttpGet]
        public IActionResult LocalLogin(string returnUrl = "/")
        {
            // Only allow local login in development
            if (!_env.IsDevelopment() && Environment.GetEnvironmentVariable("BYPASS_AUTH") != "true")
            {
                return RedirectToAction("Login", new { returnUrl });
            }

            // Pass the return URL to the view
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// POST: Process the local login form submission
        /// </summary>
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
                case "gese":
                case "secretaria":
                    claims.Add(new Claim(ClaimTypes.Role, "GESE")); // New name for Secretaria
                    claims.Add(new Claim(ClaimTypes.Role, "Secretaria")); // Keep old name for backward compatibility
                    break;
                case "gestor":
                    claims.Add(new Claim(ClaimTypes.Role, "Gestor"));
                    break;
                case "gedth":
                case "coordenador":
                    claims.Add(new Claim(ClaimTypes.Role, "GEDTH")); // New name for Coordenador
                    claims.Add(new Claim(ClaimTypes.Role, "Coordenador")); // Keep old name for backward compatibility
                    break;
                case "all":
                    // Add all roles for complete testing
                    claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
                    claims.Add(new Claim(ClaimTypes.Role, "GESE"));
                    claims.Add(new Claim(ClaimTypes.Role, "Secretaria"));
                    claims.Add(new Claim(ClaimTypes.Role, "Gestor"));
                    claims.Add(new Claim(ClaimTypes.Role, "GEDTH"));
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

            Console.WriteLine($"=== LOCAL LOGIN ATTEMPT ===");
            Console.WriteLine($"Username: {username}");
            Console.WriteLine($"Role: {role}");
            Console.WriteLine($"Return URL: {returnUrl}");

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            Console.WriteLine("✅ Login successful! Redirecting to: " + returnUrl);
            
            return LocalRedirect(returnUrl);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            if (_env.IsDevelopment() || Environment.GetEnvironmentVariable("BYPASS_AUTH") == "true")
            {
                // Local logout
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/");
            }

            // Production logout (Acesso Cidadão)
            await HttpContext.SignOutAsync("oidc", new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            });

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return Redirect("/");
        }
    }
}