using BugTracker.Extensions;
using BugTracker.Services.Interfaces;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Controllers;

[Authorize]
public class UserRolesController : Controller
{
    private readonly IRolesService _rolesService;
    private readonly ICompanyInfoService _companyInfoService;

    public UserRolesController(IRolesService rolesService, ICompanyInfoService companyInfoService)
    {
        _rolesService = rolesService;
        _companyInfoService = companyInfoService;
    }

    // GET
    public async Task<IActionResult> ManageUserRoles()
    {
        // Add an instance of the ViewModel as a list
        var model = new List<ManageUserRolesViewModel>();
        
        // Get the company id
        var companyId = User.Identity.GetCompanyId().Value;
        
        // Get all company users
        var users = await _companyInfoService.GetAllMembersAsync(companyId);

        foreach (var user in users)
        {
            ManageUserRolesViewModel viewModel = new();
            viewModel.ApplicationUser = user;
            IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
            viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);
            model.Add(viewModel);
        }
        
        // return the model to the View
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
    {
        // Get the company Id
        int companyId = User.Identity.GetCompanyId().Value;

        // Create a new instance of Application User
        var applicationUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.ApplicationUser.Id);

        // Get roles for the User
        IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(applicationUser);

        // Grab the selected role
        var userRole = member.SelectedRoles.FirstOrDefault();

        if (!string.IsNullOrEmpty(userRole))
        {
            // Remove User from their roles
            if (await _rolesService.RemoveUserFromRolesAsync(applicationUser, roles))
            {
                // Add User to the new role
                await _rolesService.AddUserToRoleAsync(applicationUser, userRole);
            }
        }

        // Navigate back to view
        return RedirectToAction(nameof(ManageUserRoles));
    }
}