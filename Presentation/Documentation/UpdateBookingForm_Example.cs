using Domain.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Documentation;

public class UpdateBookingForm_Example : IExamplesProvider<UpdateBookingForm>
{
    public UpdateBookingForm GetExamples() => new()
    {
        Id = "d1f8b2c3-4e5f-6a7b-8c9d-e0f1g2h3i4j5",
        InvoiceId = "a1b2c3d4-e5f6-7g8h-9i0j-k1l2m3n4o5p6",
        StatusId = 2,
        TicketCategoryName = "Platinum",
        TicketPrice = 1099,
        TicketQuantity = 2,
        EventId = "36e9b85e-8ed6-4180-965f-558ec5183c0a",
        UserId = "8b00bca4-136a-43f1-802f-75693a5b8ccc",
    };

};

