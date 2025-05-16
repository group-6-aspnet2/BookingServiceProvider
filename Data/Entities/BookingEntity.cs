using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class BookingEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();


    [ForeignKey(nameof(Status))]
    public int StatusId { get; set; }
    public virtual StatusEntity Status { get; set; } = null!;

    public string EventId { get; set; } = null!;
    public string? InvoiceId { get; set; }
    public string UserId { get; set; } = null!;



    public string TicketCategoryName { get; set; } = null!;

    [Column(TypeName = "money")]
    public decimal TicketPrice { get; set; }
    public int TicketQuantity { get; set; }
    
    public DateTime CreateDate { get; set; } = DateTime.Now;

}
