using Microsoft.AspNetCore.Identity;

namespace BT.Infrastructure.Helpers;

public static class DataHelper
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = ["Admin", "ProjectManager", "Developer", "Submitter", "DemoUser"];
        
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}