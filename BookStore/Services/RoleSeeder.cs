using BookStore.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Services
{
    public static class RoleSeeder
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Roles>>();

            string[] roleNames = { "Admin", "User", "Staff" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new Roles { Name = roleName });
                }
            }
        }
    }
}
