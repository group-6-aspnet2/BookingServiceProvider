using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class BookingEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreateDate { get; set; } = DateTime.Now;


    [ForeignKey(nameof(Status))]
    public int StatusId { get; set; }
    public virtual StatusEntity Status { get; set; } = null!;


    public string EventId { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public string EventCategoryName { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public string InvoiceId { get; set; } = null!;
    public int TicketCategoryId { get; set; } 

    [Column(TypeName = "money")]
    public decimal TicketPrice { get; set; }
    public int TicketQuantity { get; set; }

    [Column(TypeName = "money")]

    public decimal TotalPrice { get; set; }

    public string EVoucherId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;



}
