using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models;

public class Company
{
    public int Id { get; set; }

    [Display(Name = "Company Name")]
    public string Name { get; set; }
    
    [Display(Name = "Company Description")]
    public string Description { get; set; }
    
    // Navigation properties
    public virtual ICollection<ApplicationUser> Members { get; set; } = new HashSet<ApplicationUser>();
    public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
}