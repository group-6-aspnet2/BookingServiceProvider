using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class CreateBookingForm
{
    [Required]
    public string EventId { get; set; } = null!;
    [Required]
    public string TicketCategoryName { get; set; } = null!;
    [Required]
    public decimal TicketPrice { get; set; }
    [Required]
    public int TicketQuantity { get; set; }
    [Required]
    public decimal TotalPrice { get; set; }
    [Required]
    public string UserId { get; set; } = null!;

    // ska eventproperties skickas med eller hämtas i service med eventId?
    //public string EventName { get; set; } = null!; 
    //public string EventCategory { get; set; } = null!;
    //public DateTime Date { get; set; }



    // ska användaruppgifter skickas med eller hämtas i service med userId?
    //public string FirstName { get; set; } = null!;
    //public string LastName { get; set; } = null!;
    //public string Email { get; set; } = null!;
    //public string PhoneNumber { get; set; } = null!;

}


