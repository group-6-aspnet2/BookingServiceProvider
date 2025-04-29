namespace Domain.Models;

public class BookingModel
{
    public string Id { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public int EventCategoryId { get; set; } 
    public DateTime Date { get; set; }
    public string InvoiceId { get; set; } = null!;  // sätts när invoice skapats i sin service provider
    public int TicketCategoryId { get; set; } 
    public decimal TicketPrice { get; set; }
    public int TicketQuantity { get; set; }
    public decimal TotalPrice { get; set; }
    public int StatusId { get; set; }
    public string EVoucherId { get; set; } = null!;  // sätts när evoucher skapats i sin service provider
    public string UserId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}


public class UpdateBookingForm
{
    public string Id { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public int EventCategoryId { get; set; }
    public DateTime Date { get; set; }
    public string InvoiceId { get; set; } = null!;  // sätts när invoice skapats i sin service provider
    public int TicketCategoryId { get; set; }
    public decimal TicketPrice { get; set; }
    public int TicketQuantity { get; set; }
    public decimal TotalPrice { get; set; }
    public int StatusId { get; set; }
    public string EVoucherId { get; set; } = null!;  // sätts när evoucher skapats i sin service provider
    public string UserId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}