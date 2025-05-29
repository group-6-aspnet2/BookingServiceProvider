using Domain.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Documentation;

public class Bookings_Example : IExamplesProvider<IEnumerable<BookingModel>>
{
    public IEnumerable<BookingModel> GetExamples() => new List<BookingModel>
    {
        new BookingModel
        {
            Id = "dedb1bc2-1076-4903-ae8f-6b946a03a198",
            EventId = "36e9b85e-8ed6-4180-965f-558ec5183c0a",
            EventName = "Rock Festival 2026",
            EventCategoryName = "Music",
            EventDate = new DateOnly(2026, 7, 15),
            EventTime = new TimeOnly(18, 30),
            InvoiceId = "6934a3b6-9e57-42d0-adba-682e4ebef5c0",
            UserId = "8b00bca4-136a-43f1-802f-75693a5b8ccc",
            TicketCategoryName = "Platinum",
            TicketPrice = 1099,
            TicketQuantity = 2,
            StatusId = 1,
            StatusName = "Pending",
            FirstName = "Emma",
            LastName = "Smith",
            PhoneNumber ="11122233344455",
            Email = "emma@gmail.com",
            CreateDate = DateTime.Now
        },
        new BookingModel
        {
            Id = "c1d6b167-4f3a-4dfc-8e7c-4570250d8e02",
            EventId = "36e9b85e-8ed6-4180-965f-558ec5183c0b",
            EventName = "Rock Festival 2026",
            EventCategoryName = "Music",
            EventDate = new DateOnly(2026, 7, 15),
            EventTime = new TimeOnly(18, 30),
            InvoiceId = "c62cdeba-66a1-4b48-9e3f-4a72cfbda90d",
            UserId = "fd432e51-0a1c-447b-8ebe-f98f03a96eba",
            TicketCategoryName = "Silver",
            TicketPrice = 299,
            TicketQuantity = 1,
            StatusId = 2,
            StatusName = "Confirmed",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@domain.com",
            PhoneNumber = "987654321",
            CreateDate = DateTime.Now
        },
          new BookingModel
        {
            Id = "075af248-b265-495d-8e38-a432d473753f",
            EventId = "78082451-a445-4dcb-ab28-389e662c7591",
            EventName = "Tech Conference",
            EventCategoryName = "Technology",
            EventDate = new DateOnly(2025, 8, 20),
            EventTime = new TimeOnly(11, 0),
            InvoiceId = "8729da8c-2489-4c3e-987d-89dadcea5f3a",
            TicketCategoryName = "Platinum",
            TicketPrice = 799,
            TicketQuantity = 1,
            UserId = "8b00bca4-136a-43f1-802f-75693a5b8ccd",
            StatusId = 1,
            StatusName = "Pending",
            FirstName = "Alice",
            LastName = "Johnson",
            Email = "alice@domain.com",
            PhoneNumber = "123456789",
            CreateDate = DateTime.Now

        },

    };
}
