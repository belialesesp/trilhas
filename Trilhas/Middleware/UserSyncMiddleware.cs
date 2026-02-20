using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Trilhas.Data;
using Trilhas.Data.Model.Users;

namespace Trilhas.Middleware
{
    /// <summary>
    /// Syncs user information from Acesso Cidadão claims to local database
    /// </summary>
    public class UserSyncMiddleware
    {
        private readonly RequestDelegate _next;

        public UserSyncMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            // Only sync if user is authenticated
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.FindFirst("subnovo")?.Value;
                var name = context.User.FindFirst("name")?.Value;
                var email = context.User.FindFirst("email")?.Value;
                var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    var userProfile = dbContext.UserProfiles.Find(userId);

                    if (userProfile == null)
                    {
                        // First login - create user profile
                        userProfile = new UserProfile
                        {
                            UserId = userId,
                            Name = name,
                            Email = email,
                            Role = role,
                            FirstLogin = DateTime.Now,
                            LastLogin = DateTime.Now,
                            ReceiveNotifications = true
                        };
                        dbContext.UserProfiles.Add(userProfile);
                    }
                    else
                    {
                        // Update existing profile
                        userProfile.Name = name;
                        userProfile.Email = email;
                        userProfile.Role = role;
                        userProfile.LastLogin = DateTime.Now;
                    }

                    await dbContext.SaveChangesAsync();
                }
            }

            await _next(context);
        }
    }
}