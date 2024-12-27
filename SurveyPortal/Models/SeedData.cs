using Microsoft.AspNetCore.Identity;

namespace SurveyPortal.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Rolleri oluştur
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // Yönetici kullanıcı oluştur
            var adminUser = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com" };
            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
