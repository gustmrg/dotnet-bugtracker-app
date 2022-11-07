using System.Security.Claims;
using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BugTracker.Services.Factories;

public class BTUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public BTUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IOptions<IdentityOptions> options) 
        : base(userManager, roleManager, options)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("CompanyId", user.CompanyId.ToString()));
        return identity;
    }
}