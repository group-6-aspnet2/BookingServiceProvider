using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Presentation.AzureFunctions.Functions.RestApis;

public class GetBookingsFunction
{
    private readonly ILogger<GetBookingsFunction> _logger;
    private readonly IBookingService _bookingService;

    public GetBookingsFunction(ILogger<GetBookingsFunction> logger, IBookingService bookingService)
    {
        _logger = logger;
        _bookingService = bookingService;
    }

    [Function("GetBookingsFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "bookings")] HttpRequest req)
    {
        var result =await  _bookingService.GetAllBookingsAsync();

        return result.StatusCode switch
        {
            200 => new OkObjectResult(result.Result),
            _ => new ObjectResult(result.Error) { StatusCode = result.StatusCode }
        };
    }
}