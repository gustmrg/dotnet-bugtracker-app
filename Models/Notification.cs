using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models;

public class Notification
{
    public int Id { get; set; }

    [Display(Name = "Ticket")]
    public int TicketId { get; set; }

    [Required]
    [Display(Name = "Title")]
    public string Title { get; set; }
    
    [Required]
    [Display(Name = "Message")]
    public string Message { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Created Date")]
    public DateTimeOffset Created { get; set; }

    [Required]
    [Display(Name = "Recipient")]
    public string RecipientId { get; set; }
    
    [Required]
    [Display(Name = "Sender")]
    public string SenderId { get; set; }

    [Display(Name = "Has Been Viewed")]
    public bool Viewed { get; set; }
    
    // Navigation properties
    public virtual Ticket Ticket { get; set; }
    public virtual ApplicationUser Recipient { get; set; }
    public virtual ApplicationUser Sender { get; set; }
}