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

            IdentityUser user = await userManager.FindByEmailAsync("carterleitch@gmail.com");
            if (user != null)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
