using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SportsStore.Models
{
    public static class IdentitySeedData
    {
        private const string AdminUser = "Admin";
        private const string AdminPassword = "Secret123$";

        public static async Task EnsurePopulated(IApplicationBuilder app)
        {
            if (app == null) 
            {
                throw new ArgumentNullException(nameof(app));
            }

            using var serviceProvider = app.ApplicationServices.CreateScope();
            AppIdentityDbContext appIdentityDb = serviceProvider.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
            if (appIdentityDb.Database.GetMigrations().Any())
            {
                appIdentityDb.Database.Migrate();
            }

            UserManager<IdentityUser> userManager = serviceProvider.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            IdentityUser user = await userManager.FindByNameAsync(AdminUser);
            if (user is null)
            {
                user = new IdentityUser("Admin")
                {
                    Email = "admin@example.com",
                    PhoneNumber = "555-1234",
                };

                await userManager.CreateAsync(user, AdminPassword);
            }
        }
    }
}
