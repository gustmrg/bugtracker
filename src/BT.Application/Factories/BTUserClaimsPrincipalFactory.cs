using System.Security.Claims;
using BT.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BT.Application.Factories;

public class BTUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> options)
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(userManager, roleManager, options)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var identity = await base.GenerateClaimsAsync(user);
        
        // Add company claim with proper null checking
        // if (user.CompanyId.HasValue)
        // {
        //     identity.AddClaim(new Claim(CustomClaimTypes.CompanyId, user.CompanyId.Value.ToString()));
        // }
        
        if (!string.IsNullOrEmpty(user.FirstName))
        {
            identity.AddClaim(new Claim(CustomClaimTypes.FirstName, user.FirstName));
        }
        
        if (!string.IsNullOrEmpty(user.LastName))
        {
            identity.AddClaim(new Claim(CustomClaimTypes.LastName, user.LastName));
        }
        
        if (!string.IsNullOrEmpty(user.FullName))
        {
            identity.AddClaim(new Claim(CustomClaimTypes.FullName, user.FullName));
        }
        
        // Add avatar indicator
        // if (!string.IsNullOrEmpty(user.AvatarFileName))
        // {
        //     identity.AddClaim(new Claim(CustomClaimTypes.HasAvatar, "true"));
        // }
        
        return identity;
    }
}

public static class CustomClaimTypes
{
    public const string CompanyId = "CompanyId";
    public const string FullName = "FullName";
    public const string FirstName = "FirstName";
    public const string LastName = "LastName";
    public const string HasAvatar = "HasAvatar";
}