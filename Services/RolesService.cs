using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services;

public class RolesService : IRolesService
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RolesService(ApplicationDbContext context, 
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<bool> IsUserInRoleAsync(ApplicationUser user, string roleName)
    {
        bool result = await _userManager.IsInRoleAsync(user, roleName);
        return result;
    }

    public async Task<List<IdentityRole>> GetRolesAsync()
    {
        try
        {
            List<IdentityRole> result = new();
            result = await _context.Roles.ToListAsync();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(ApplicationUser user)
    {
        IEnumerable<string> result = await _userManager.GetRolesAsync(user);
        return result;
    }

    public async Task<bool> AddUserToRoleAsync(ApplicationUser user, string roleName)
    {
        bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;
        return result;
    }

    public async Task<bool> RemoveUserFromRoleAsync(ApplicationUser user, string roleName)
    {
        bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;
        return result;
    }

    public async Task<bool> RemoveUserFromRolesAsync(ApplicationUser user, IEnumerable<string> roles)
    {
        bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;
        return result;
    }

    public async Task<List<ApplicationUser>> GetUsersInRoleAsync(string roleName, int companyId)
    {
        List<ApplicationUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
        List<ApplicationUser> result = users.Where(u => u.CompanyId == companyId).ToList();
        return result;
    }

    public async Task<List<ApplicationUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
    {
        List<string> userIds = (await _userManager.GetUsersInRoleAsync(roleName)).Select(u => u.Id).ToList();
        List<ApplicationUser> roleUsers = _context.Users.Where(u => !userIds.Contains(u.Id)).ToList();
        List<ApplicationUser> result = roleUsers.Where(u => u.CompanyId == companyId).ToList();
        return result;
    }

    public async Task<string> GetRoleNameByIdAsync(string roleId)
    {
        IdentityRole role = _context.Roles.Find(roleId);
        string result = await _roleManager.GetRoleNameAsync(role);
        return result;
    }
}