namespace Domain.Models;

public class BookingConfirmationRequest
{
    public string BookingId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
}
