using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Presentation.AzureFunctions.Functions.RestApis;

public class GetBookingsByUserFunction
{
    private readonly ILogger<GetBookingsByUserFunction> _logger;
    private readonly IBookingService _bookingService;


    public GetBookingsByUserFunction(ILogger<GetBookingsByUserFunction> logger, IBookingService bookingService)
    {
        _logger = logger;
        _bookingService = bookingService;
    }

    [Function("GetBookingsByUserFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route ="bookings/user/{userId}")] HttpRequest req, string userId)
    {
        var result = await _bookingService.GetBookingsByUserIdAsync(userId);

        return result.StatusCode switch
        {
            200 => new OkObjectResult(result.Result),
            404 => new NotFoundObjectResult(result.Error),
            _ => new ObjectResult(result.Error) { StatusCode = result.StatusCode }
        };
    }
}