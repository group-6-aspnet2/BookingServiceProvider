namespace Domain.Models;

public class CreateBookingRequest
{
    public string EventId { get; set; } = null!;
    public string TicketCategoryName { get; set; } = null!;
    public decimal TicketPrice { get; set; }
    public int TicketQuantity { get; set; }
    public int StatusId { get; set; }
    public string UserId { get; set; } = null!;
}
