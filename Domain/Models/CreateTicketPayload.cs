using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public class CreateTicketPayload
{
    public string BookingId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public int TicketQuantity { get; set; }
    [Column(TypeName = "money")]
    public decimal TicketPrice { get; set; }
    public string TicketCategoryName { get; set; } = null!;

}
