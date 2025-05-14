using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class UpdateBookingForm
{
    [Required]
    public string Id { get; set; } = null!;
    [Required]
    public string EventId { get; set; } = null!;
    [Required]
    public string InvoiceId { get; set; } = null!;
    [Required]
    public string TicketCategoryName { get; set; } = null!;
    [Required]
    public decimal TicketPrice { get; set; }
    [Required]
    public int TicketQuantity { get; set; }
    [Required]
    public int StatusId { get; set; }
    [Required]
    public string UserId { get; set; } = null!;
}