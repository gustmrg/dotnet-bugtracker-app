using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models;

public class TicketAttachment
{
    public int Id { get; set; }

    [Display(Name = "Ticket")]
    public int TicketId { get; set; }

    [Display(Name = "File Date")]
    public DateTimeOffset Created { get; set; }

    [Display(Name = "Team Member")]
    public string UserId { get; set; }

    [Display(Name = "File Description")]
    public string Description { get; set; }

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile FormFile { get; set; }
    
    [Display(Name = "File Name")]
    public string FileName { get; set; }
    public byte[] FileData { get; set; }
    
    [Display(Name = "File Extension")]
    public string FileContentType { get; set; }
    
    // Navigation properties
    public virtual Ticket Ticket { get; set; }
    public virtual ApplicationUser User { get; set; }
}