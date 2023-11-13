using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp.Deserializers;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Trilhas.Data;
using Trilhas.Data.Model;
using Trilhas.Helper;
using Trilhas.Services;
using Trilhas.Settings;

namespace Trilhas
{
	public class Startup
	{
		private readonly ILogger<Startup> _logger;

		public Startup(ILogger<Startup> logger, IConfiguration configuration)
		{
			_logger = logger;
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options => {
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.Configure<RequestLocalizationOptions>(options => {
				options.DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR");
			});

			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // SETTINGS
            services.Configure<MinioSettings>(Configuration.GetSection("MinioSettings"));

			// SERVICES 
			services.AddSingleton<MinioService>();
			services.AddSingleton<FileHelper>();
			services.AddScoped<RelatorioService>();
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

            services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = "oidc";
			})
			.AddCookie(options => {
				options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
				options.Cookie.Name = "TrilhasCookie";
				options.AccessDeniedPath = "/Error/AccessDenied";

				options.Events = new CookieAuthenticationEvents {
					OnSignedIn = async context => {
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
			.AddOpenIdConnect("oidc", options => {
				options.Authority = Configuration["OpenIdSettings:Authority"];
				options.ClientId = Configuration["OpenIdSettings:ClientId"];
				options.ClientSecret = Configuration["OpenIdSettings:ClientSecret"];
				options.SaveTokens = Configuration.GetValue<bool>("OpenIdSettings:SaveTokens");
				options.CallbackPath = new PathString(Configuration["OpenIdSettings:CallbackPath"]);
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

				options.Events = new OpenIdConnectEvents() {
					OnUserInformationReceived = async c => {
						var id = new ClaimsIdentity(c.Principal.Identity.AuthenticationType);

                        var user = JsonConvert.DeserializeObject<Usuario>(c.User.RootElement.ToString());

                        id.AddClaim(new Claim("verificada", user.Verificada.ToString()));

                        if (!string.IsNullOrEmpty(user.Apelido))
						{
							id.AddClaim(new Claim(id.NameClaimType, user.Apelido));
						}

						if(!string.IsNullOrEmpty(user.Email))
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

				options.TokenValidationParameters = new TokenValidationParameters {
					NameClaimType = JwtClaimTypes.Name,
					RoleClaimType = JwtClaimTypes.Role,
				};
			});

			services.AddMvc(options => {
				options.EnableEndpointRouting = false;
			}).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if(env.IsDevelopment())
			{
				app.UseHttpsRedirection();
				app.UseDeveloperExceptionPage();
			} else
			{
				app.Use((context, next) => {
					context.Request.Scheme = "https";
					return next();
				});
				app.UseExceptionHandler("/Home/Error");
			}

			//app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseAuthentication();

			app.UseMvc(routes => {
				routes.MapRoute("admin", "admin/{*index}", defaults: new { controller = "Admin", action = "Index" });
				routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
