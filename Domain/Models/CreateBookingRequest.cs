namespace Domain.Models;

public class CreateBookingRequest
{
    // hämta eventproperties med eventId
    public string EventId { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public string EventCategory { get; set; } = null!;
    public DateTime Date { get; set; }


    public string TicketCategory { get; set; } = null!;
    public decimal TicketPrice { get; set; }
    public int TicketQuantity { get; set; }
    public decimal TotalPrice { get; set; }
    public int StatusId { get; set; }

    // hämta användaruppgifter med userId
    public string UserId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}
