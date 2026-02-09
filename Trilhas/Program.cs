using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Trilhas.Data;

namespace Trilhas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            // Apply database migrations on startup (non-fatal)
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    
                    logger.LogInformation("🔄 Checking database...");
                    
                    var pendingMigrations = context.Database.GetPendingMigrations().ToList();
                    
                    if (pendingMigrations.Any())
                    {
                        logger.LogInformation($"📦 Found {pendingMigrations.Count} pending migrations");
                        context.Database.Migrate();
                        logger.LogInformation("✅ Migrations applied successfully!");
                    }
                    else
                    {
                        logger.LogInformation("✅ Database is up to date!");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "⚠️ Migration check failed - continuing anyway. Database may need manual migration.");
                    // DON'T throw - let the app start anyway
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}