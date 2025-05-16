using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Presentation.AzureFunctions.Functions.RestApis;

public class GetBookingByIdFunction
{
    private readonly ILogger<GetBookingByIdFunction> _logger;
    private readonly IBookingService _bookingService;
    public GetBookingByIdFunction(ILogger<GetBookingByIdFunction> logger, IBookingService bookingService)
    {
        _logger = logger;
        _bookingService = bookingService;
    }

    [Function("GetBookingByIdFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "bookings/{id}")] HttpRequest req, string id)
    {
        var result = await _bookingService.GetOneAsync(id);

        return result.StatusCode switch
        {
            200 => new OkObjectResult(result.Result),
            404 => new NotFoundObjectResult(result.Error),
            _ => new ObjectResult(result.Error) { StatusCode = result.StatusCode }
        };
    }
}