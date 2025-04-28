namespace Domain.Models;

public class CreateBookingForm
{
        public string EventId { get; set; } = null!;
        public string TicketCategory { get; set; } = null!;
        public decimal TicketPrice { get; set; }
        public int TicketQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int StatusId { get; set; }       
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
