using Microsoft.AspNetCore.Identity;
using ProjectTube.Constants;

namespace ProjectTube.Data;

public class UserSeeder
{
    public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        await CreateUserWithRoles(userManager, "admin", "admin@localhost", "Admin!23", Roles.Admin);
        await CreateUserWithRoles(userManager, "founder", "founder@localhost", "Founder!23", Roles.Admin);
        await CreateUserWithRoles(userManager, "user", "user@localhost", "User!23", Roles.User);
    }
    
    
    
    
    private static async Task CreateUserWithRoles(UserManager<IdentityUser> userManager, string username, string email,
        string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new IdentityUser
            {
                Email = email,
                UserName = username,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
            else
            {
                throw new Exception($"failed to create user with {user.Email}. Errors: {string.Join(",", result.Errors)}");
            }
            
        }
    }
}