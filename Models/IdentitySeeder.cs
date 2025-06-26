using Microsoft.AspNetCore.Identity;

namespace BakingIt.Models
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
        {
            var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userMgr = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Seed Roles
            foreach (var roleName in new[] { "Admin", "Guest", "Baker" })
            {
                if (!await roleMgr.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName)
                    {
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    };
                    await roleMgr.CreateAsync(role);
                }
            
            }

            // 2. Seed Default Admin
            var adminEmail = config["SeedAdmin:Email"];
            var adminPassword = config["SeedAdmin:Password"];
            if (await userMgr.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
                var result = await userMgr.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userMgr.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
