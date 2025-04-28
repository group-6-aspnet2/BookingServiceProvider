namespace Data.Entities;

public class BookingEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EventId { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public string EventCategory { get; set; } = null!;
    public DateTime Date { get; set; }
    public string InvoiceId { get; set; } = null!;
    public string TicketCategory { get; set; } = null!;
    public decimal TicketPrice { get; set; }
    public int TicketQuantity { get; set; }
    public decimal TotalPrice { get; set; }
    public int StatusId { get; set; } 
    public string EVoucherId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

}
