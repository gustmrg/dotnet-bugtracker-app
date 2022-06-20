using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models;

public class ProjectPriority
{
    public int Id { get; set; }

    [Display(Name = "Priority Name")]
    public string Name { get; set; }
}