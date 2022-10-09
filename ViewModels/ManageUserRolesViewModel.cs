using BugTracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public MultiSelectList Roles { get; set; }
        public List<string> SelectedRoles { get; set; }
    }
}
