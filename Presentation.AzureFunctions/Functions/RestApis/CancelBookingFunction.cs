using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Presentation.AzureFunctions.Functions.RestApis;

public class CancelBookingFunction
{
    private readonly ILogger<CancelBookingFunction> _logger;
    private readonly IBookingService _bookingService;

    public CancelBookingFunction(ILogger<CancelBookingFunction> logger, IBookingService bookingService)
    {
        _logger = logger;
        _bookingService = bookingService;
    }

    [Function("CancelBookingFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,"delete", Route = "bookings/{id}") ] HttpRequest req, string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                return new BadRequestObjectResult(new { message = "No booking id provided" });

            var cancelResult = await _bookingService.CancelBookingAsync(id);
            return cancelResult.StatusCode switch
            {
                204 => new NoContentResult(),
                400 => new BadRequestObjectResult(cancelResult.Error),
                404 => new NotFoundObjectResult(cancelResult.Error),
                _ => new ObjectResult(cancelResult.Error),
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new ObjectResult(ex.Message);
        }
    }
}