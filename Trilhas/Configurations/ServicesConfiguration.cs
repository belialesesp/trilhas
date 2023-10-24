using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Trilhas.Data.Model;
using Trilhas.Services;
using Trilhas.Settings;

namespace Trilhas.Configurations
{
    public static class ServicesConfiguration
    {
        public static void AddAuthentication(this IServiceCollection services, OpenIdService settings)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie.Name = "TrilhasCookie";
                options.AccessDeniedPath = "/Error/AccessDenied";

                options.Events = new CookieAuthenticationEvents
                {
                    OnSignedIn = async context =>
                    {
                        Guid idUsuario = context.Principal.Claims
                            .Where(c => c.Type.ToLower().Equals("subnovo"))
                            .Select(c => new Guid(c.Value))
                            .FirstOrDefault();

                        IServiceProvider serviceProvider = context.HttpContext.RequestServices;
                        IDistributedCache distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();

                        await distributedCache.RemoveAsync(idUsuario.ToString());
                    },
                };
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = settings.Authority;
                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;
                options.SaveTokens = settings.SaveTokens;
                options.CallbackPath = new PathString(settings.CallbackPath);
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = true;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                //options.Scope.Add("cpf");
                options.Scope.Add("permissoes");
                options.Scope.Add("agentepublico");

                options.Events = new OpenIdConnectEvents()
                {
                    OnUserInformationReceived = async c =>
                    {
                        var id = new ClaimsIdentity(c.Principal.Identity.AuthenticationType);

                        var user = JsonConvert.DeserializeObject<Usuario>(c.User.RootElement.ToString());

                        id.AddClaim(new Claim("verificada", user.Verificada.ToString()));

                        if (!string.IsNullOrEmpty(user.Apelido))
                        {
                            id.AddClaim(new Claim(id.NameClaimType, user.Apelido));
                        }

                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            id.AddClaim(new Claim("email", user.Email));
                        }

                        if (!string.IsNullOrEmpty(user.Role))
                        {
                            id.AddClaim(new Claim(id.RoleClaimType, user.Role));
                        }

                        id.AddClaim(new Claim("id_token", c.ProtocolMessage.IdToken));
                        c.Principal.AddIdentity(id);
                        c.Success();

                        await Task.FromResult(0);
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role,
                };
            });
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<MinioService>();
            services.AddSingleton<OpenIdService>();
            services.AddScoped<TrilhasService>();
            services.AddScoped<CadastroService>();
            services.AddScoped<EixoService>();
            services.AddScoped<EstacaoService>();
            services.AddScoped<SolucaoEducacionalService>();
            services.AddScoped<LocalService>();
            services.AddScoped<EventoService>();
            services.AddScoped<PessoaService>();
            services.AddScoped<RecursoService>();
            services.AddScoped<EntidadeService>();
            services.AddScoped<DocenteService>();
            services.AddScoped<ListaPresencaService>();
            services.AddScoped<DocenteService>();
            services.AddScoped<CertificadoService>();
            services.AddScoped<CertificadoEmitidoService>();
        }

        public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MinioSettings>(configuration.GetSection("MinioSettings"));
            services.Configure<OpenIdSettings>(configuration.GetSection("OpenIdSettings"));
        }
    }
}
