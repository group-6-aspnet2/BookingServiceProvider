using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Presentation.AzureFunctions.Functions.RestApis;

public class GetBookingsByStatusFunction
{
    private readonly ILogger<GetBookingsByStatusFunction> _logger;
    private readonly IBookingService _bookingService;

    public GetBookingsByStatusFunction(ILogger<GetBookingsByStatusFunction> logger, IBookingService bookingService)
    {
        _logger = logger;
        _bookingService = bookingService;
    }

    [Function("GetBookingsByStatusFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "bookings/status/{statusId}")] HttpRequest req, int statusId)
    {
        var result = await _bookingService.GetBookingsByStatusIdAsync(statusId);

        return result.StatusCode switch
        {
            200 => new OkObjectResult(result.Result),
            _ => new ObjectResult(result.Error) { StatusCode = result.StatusCode }
        };
    }
}