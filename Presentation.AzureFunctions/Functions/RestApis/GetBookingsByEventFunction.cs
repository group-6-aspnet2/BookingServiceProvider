using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Presentation.AzureFunctions.Functions.RestApis;

public class GetBookingsByEventFunction
{
    private readonly ILogger<GetBookingsByEventFunction> _logger;
    private readonly IBookingService _bookingService;

    public GetBookingsByEventFunction(ILogger<GetBookingsByEventFunction> logger, IBookingService bookingService)
    {
        _logger = logger;
        _bookingService = bookingService;
    }

    [Function("GetBookingsByEventFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "bookings/event/{eventId}")] HttpRequest req, string eventId)
    {
        var result = await _bookingService.GetBookingsByEventIdAsync(eventId);

        return result.StatusCode switch
        {
            200 => new OkObjectResult(result.Result),
            404 => new NotFoundObjectResult(result.Error),
            _ => new ObjectResult(result.Error) { StatusCode = result.StatusCode }
        };
    }
}