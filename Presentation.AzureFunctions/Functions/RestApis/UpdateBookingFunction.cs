using Business.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Presentation.AzureFunctions.Helpers;
using System.Diagnostics;

namespace Presentation.AzureFunctions.Functions.RestApis;

public class UpdateBookingFunction
{
    private readonly ILogger<UpdateBookingFunction> _logger;
    private readonly IBookingService _bookingService;

    public UpdateBookingFunction(ILogger<UpdateBookingFunction> logger, IBookingService bookingService)
    {
        _logger = logger;
        _bookingService = bookingService;
    }

    [Function("UpdateBookingFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "bookings")] HttpRequest req)
    {
        try
        {
            var form = await AzureFunctionHelpers.GetModelFromBody<UpdateBookingForm>(req);

            if (form == null)
                return new BadRequestObjectResult("No booking form provided");

            if (string.IsNullOrWhiteSpace(form.UserId) || form.StatusId == 0 || string.IsNullOrWhiteSpace(form.InvoiceId) || string.IsNullOrWhiteSpace(form.Id) || string.IsNullOrWhiteSpace(form.EventId) || string.IsNullOrWhiteSpace(form.TicketCategoryName) || form.TicketPrice <= 0 || form.TicketQuantity <= 0)
                return new BadRequestObjectResult("All required fields were not provided");

            var updateResult = await _bookingService.UpdateBookingAsync(form);
            return updateResult.StatusCode switch
            {
                200 => new OkObjectResult(updateResult.Result),
                400 => new BadRequestObjectResult(updateResult.Error),
                404 => new NotFoundObjectResult(updateResult.Error),
                _ => new ObjectResult(updateResult.Error) { StatusCode = updateResult.StatusCode}
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new ObjectResult(ex.Message);
        }

    }
}