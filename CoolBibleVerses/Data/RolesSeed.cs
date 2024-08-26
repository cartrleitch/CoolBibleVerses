using CoolBibleVerses.Models;
using Microsoft.AspNetCore.Identity;

namespace CoolBibleVerses.Data
{
    public class RolesSeed
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "User" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Create the roles and seed them to the database
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            IdentityUser admin1 = await userManager.FindByEmailAsync("carterleitch@gmail.com");
            IdentityUser admin2 = await userManager.FindByEmailAsync("yourhotmom12@gmail.com");

            if (admin1 != null)
            {
                await userManager.AddToRoleAsync(admin1, "Admin");
            }
            if (admin2 != null)
            {
                await userManager.AddToRoleAsync(admin2, "Admin");
            }
        }
    }
}
