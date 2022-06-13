using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Required]
    [Display(Name = "First Name")]
    public string LastName { get; set; }
    
    [NotMapped]
    [Display(Name = "First Name")]
    public string FullName => $"{FirstName} {LastName}";
}