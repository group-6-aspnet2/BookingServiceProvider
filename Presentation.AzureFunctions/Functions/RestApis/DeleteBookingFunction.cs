using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Presentation.AzureFunctions.Functions.RestApis;

public class DeleteBookingFunction
{
    private readonly ILogger<DeleteBookingFunction> _logger;
    private readonly IBookingService _bookingService;

    public DeleteBookingFunction(ILogger<DeleteBookingFunction> logger, IBookingService bookingService)
    {
        _logger = logger;
        _bookingService = bookingService;
    }

    [Function("DeleteBookingFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route ="bookings/delete/{id}")] HttpRequest req, string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                return new BadRequestObjectResult(new { message = "No booking id provided" });

            var deleteResult = await _bookingService.DeleteBookingWithTickets(id);
            return deleteResult.StatusCode switch
            {
                204 => new NoContentResult(),
                400 => new BadRequestObjectResult(deleteResult.Error),
                404 => new NotFoundObjectResult(deleteResult.Error),
                _ => new ObjectResult(deleteResult.Error),
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new ObjectResult(ex.Message);
        }
    }
}