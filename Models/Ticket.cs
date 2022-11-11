using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeStyle;

namespace BugTracker.Models;

public class Ticket
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Title")]
    [StringLength(50)]
    public string Title { get; set; }

    [Required]
    [Display(Name = "Description")]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Created")]
    [DataType(DataType.Date)]
    public DateTimeOffset Created { get; set; }

    [Display(Name = "Updated")]
    [DataType(DataType.Date)]
    public DateTimeOffset? Updated { get; set; }

    [Display(Name = "Archived")]
    public bool Archived { get; set; }
    
    [Display(Name = "Archived by Project")]
    public bool ArchivedByProject { get; set; }

    [Display(Name = "Project")]
    public int ProjectId { get; set; }
    
    [Display(Name = "Ticket Type")]
    public int TicketTypeId { get; set; }
    
    [Display(Name = "Ticket Priority")]
    public int TicketPriorityId { get; set; }
    
    [Display(Name = "Ticket Status")]
    public int TicketStatusId { get; set; }
    
    [Display(Name = "Ticket Owner")]
    public string OwnerUserId { get; set; }
    
    [Display(Name = "Ticket Developer")]
    public string DeveloperUserId { get; set; }
    
    // Navigation properties
    public virtual Project Project { get; set; }
    public virtual TicketType TicketType { get; set; }
    public virtual TicketPriority TicketPriority { get; set; }
    public virtual TicketStatus TicketStatus { get; set; }
    public virtual ApplicationUser OwnerUser { get; set; }
    public virtual ApplicationUser DeveloperUser { get; set; }

    public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
    public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
    public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
}