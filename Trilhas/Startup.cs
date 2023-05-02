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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Trilhas.Data;
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
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));
			services.AddDefaultIdentity<IdentityUser>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			// SETTINGS
			services.Configure<MinioSettings>(Configuration.GetSection("MinioSettings"));

			// SERVICES 
			services.AddSingleton<MinioService>();
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

                //options.Authority = acessoCidadaoHybridClientConfiguration.Authority;
                //options.ClientId = acessoCidadaoHybridClientConfiguration.ClientId;
                //options.ClientSecret = acessoCidadaoHybridClientConfiguration.ClientSecret;
                //options.GetClaimsFromUserInfoEndpoint = acessoCidadaoHybridClientConfiguration.GetClaimsFromUserInfoEndpoint;
                //options.RequireHttpsMetadata = acessoCidadaoHybridClientConfiguration.RequireHttpsMetadata;
                //options.ResponseType = acessoCidadaoHybridClientConfiguration.ResponseType;
                //options.SaveTokens = acessoCidadaoHybridClientConfiguration.SaveTokens;

				//ICollection<string> scopes = acessoCidadaoHybridClientConfiguration.Scopes;
				//options.Scope.Clear();

				//if (scopes != null && scopes.Count > 0)
				//{
				//    foreach (string scope in scopes)
				//    {
				//        options.Scope.Add(scope);
				//    }
				//}

				options.Events = new OpenIdConnectEvents() {
					OnUserInformationReceived = async c => {
						var id = new ClaimsIdentity(c.Principal.Identity.AuthenticationType);

						id.AddClaim(c.User.ToClaims().FirstOrDefault(uc => uc.Type.ToLower().Equals("verificada")));

						var nameClaim = c.User.ToClaims().FirstOrDefault(uc => uc.Type.ToLower().Equals("apelido"));

						if(nameClaim != null)
						{
							id.AddClaim(new Claim(id.NameClaimType, nameClaim.Value));
						}

						var emailClaim = c.User.ToClaims().FirstOrDefault(uc => uc.Type.ToLower().Equals("email"));

						if(emailClaim != null)
						{
							id.AddClaim(new Claim("email", emailClaim.Value));
						}

                        var cpfClaim = c.User.ToClaims().FirstOrDefault(uc => uc.Type.ToLower().Equals("cpf"));

                        if (cpfClaim != null)
                        {
                            id.AddClaim(new Claim("cpf", cpfClaim.Value));
                        }

                        var roles = c.User.ToClaims().Where(uc => uc.Type.ToLower().Equals("role"));

						foreach(var role in roles)
						{
							id.AddClaim(new Claim(id.RoleClaimType, role.Value));
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

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if(env.IsDevelopment())
			{
				app.UseHttpsRedirection();
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			} else
			{
				app.Use((context, next) => {
					context.Request.Scheme = "https";
					return next();
				});
				app.UseExceptionHandler("/Home/Error");
				//app.UseHsts();
			}

			//app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseAuthentication();

			app.UseMvc(routes => {
				//routes
				//.MapRoute("admin", "Admin", defaults: new { controller = "Admin", action = "Index" });

				routes.MapRoute("admin", "admin/{*index}", defaults: new { controller = "Admin", action = "Index" });
				routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");

				//.MapRoute(
				//    name: "default",
				//    template: "{controller=Home}/{action=Index}/{id?}");
				//.MapRoute(
				//    name: "defaultAdmin",
				//    template: "Admin/{action=Index}/{id?}"
				//    );
			});
		}
	}
}
