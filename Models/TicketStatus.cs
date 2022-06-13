using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models;

public class TicketStatus
{
    public int Id { get; set; }
    
    [Display(Name = "Status Name")]
    public string Name { get; set; }
}