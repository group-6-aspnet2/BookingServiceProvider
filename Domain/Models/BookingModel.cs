namespace Domain.Models;

public class BookingModel
{
    public string Id { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public string EventCategoryName { get; set; } = null!;
    public DateOnly EventDate { get; set; }
    public TimeOnly EventTime { get; set; }

    public string InvoiceId { get; set; } = null!;  // sätts när invoice skapats i sin service provider
    public string TicketCategoryName { get; set; } = null!;
    public decimal TicketPrice { get; set; }
    public int TicketQuantity { get; set; }
    public int StatusId { get; set; }
    public string StatusName { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime CreateDate { get; set; }

}
