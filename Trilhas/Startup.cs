using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Trilhas.Configurations;
using Trilhas.Data;
using Trilhas.Services;
using Trilhas.Middleware;
using Hangfire;
using Hangfire.SqlServer;

namespace Trilhas
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        private readonly IWebHostEnvironment _env;

        public Startup(ILogger<Startup> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _logger = logger;
            Configuration = configuration;
            _env = env;
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

            Console.WriteLine("🔗 Using connection string: " + Configuration.GetConnectionString("DefaultConnection"));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()
                )
            );

            // Add Identity services (required by views that inject UserManager/SignInManager)
            // But DON'T call AddIdentity() - it would override our authentication configuration
            services.AddIdentityCore<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<IdentityUser>>();

            services.AddSettings(Configuration);
            services.AddHttpClients();
            services.AddServices();

            // FIXED: Always use ServicesConfiguration.AddAuthentication for consistent auth setup
            // Build service provider to get OpenIdService (it's registered in AddServices as a singleton)
            var sp = services.BuildServiceProvider();
            var openIdService = sp.GetService<OpenIdService>();
            
            // Call the authentication extension method with environment parameter
            services.AddAuthentication(openIdService, _env);

            services.AddMvc(options => {
                options.EnableEndpointRouting = false;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<TermoReferenciaService>();
            services.AddScoped<NotificationService>();

            // Add Hangfire services - CORRECT LOCATION (in ConfigureServices)
            services.AddHangfire(config => 
                config.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseHttpsRedirection();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.Use((context, next) => {
                    context.Request.Scheme = "https";
                    return next();
                });
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            
            // Add user sync middleware AFTER authentication
            app.UseMiddleware<UserSyncMiddleware>();
            
            app.UseMvc(routes => {
                routes.MapRoute("admin", "admin/{*index}", defaults: new { controller = "Admin", action = "Index" });
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            // Configure Hangfire dashboard - CORRECT LOCATION (in Configure)
            app.UseHangfireDashboard("/hangfire");

            // Schedule the recurring job
            RecurringJob.AddOrUpdate<TermoReferenciaService>(
                "check-upcoming-courses",
                service => service.EnviarNotificacoesContratacao(),
                Cron.Daily(8)); // Run at 8 AM daily
        }
    }
}