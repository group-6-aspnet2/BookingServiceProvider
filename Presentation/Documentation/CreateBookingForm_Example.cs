using Domain.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Documentation;

public class CreateBookingForm_Example : IExamplesProvider<CreateBookingForm>
{
    public CreateBookingForm GetExamples() => new()
    {
        TicketCategoryName = "Platinum",
        TicketPrice = 1099,
        TicketQuantity = 2,
        EventId = "36e9b85e-8ed6-4180-965f-558ec5183c0a",
        UserId = "8b00bca4-136a-43f1-802f-75693a5b8ccc",
    };
}
