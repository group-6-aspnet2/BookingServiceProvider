using Business.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Presentation.AzureFunctions.Helpers;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Presentation.AzureFunctions.Functions.RestApis;

public class AddNewBookingFunction
{
    private readonly ILogger<AddNewBookingFunction> _logger;
    private readonly IBookingService _bookingService;

    public AddNewBookingFunction(ILogger<AddNewBookingFunction> logger, IBookingService bookingService)
    {
        _logger = logger;
        _bookingService = bookingService;
    }

    [Function("AddNewBookingFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "bookings")] HttpRequest req)
    {
        try
        {
            var form = await AzureFunctionHelpers.GetModelFromBody<CreateBookingForm>(req);

            if (form == null)
                return new BadRequestObjectResult("No booking form provided");

            if (string.IsNullOrWhiteSpace(form.UserId) || string.IsNullOrWhiteSpace(form.EventId) || string.IsNullOrWhiteSpace(form.TicketCategoryName) || form.TicketPrice <= 0 || form.TicketQuantity <= 0)
                return new BadRequestObjectResult("All required fields were not provided");

            var createResult = await _bookingService.CreateNewBookingAsync(form);
            return createResult.StatusCode switch
            {
                201 => new CreatedResult("booking", createResult.Result),
                400 => new BadRequestObjectResult(createResult.Error),
                404 => new NotFoundObjectResult(createResult.Error),
                _ => new ObjectResult(createResult.Error) { StatusCode = createResult.StatusCode }
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new ObjectResult(ex.Message);
        }
    }
}