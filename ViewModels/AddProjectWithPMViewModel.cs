using BugTracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.ViewModels;

public class AddProjectWithPMViewModel
{
    public Project Project { get; set; }
    public SelectList PMList { get; set; }
    public string PMId { get; set; }
    public SelectList PriorityList { get; set; }
}