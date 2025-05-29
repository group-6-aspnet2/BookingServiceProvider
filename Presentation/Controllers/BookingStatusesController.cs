using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingStatusesController(IBookingStatusService bookingStatusService) : ControllerBase
{
    private readonly IBookingStatusService _bookingStatusService = bookingStatusService;

    [HttpGet]
    [SwaggerOperation(Summary = "Returns a list of booking statuses")]
    [SwaggerResponse(200, "Statuses received successfully.")]
    [SwaggerResponse(400, "Bad request.")]
    [SwaggerResponse(404, "Statuses not found.")]
    [SwaggerResponse(500, "An error occurred while retrieving the statuses.")]
    public async Task<IActionResult> GetAllStatuses()
    {
        try
        {
            var result = await _bookingStatusService.GetAllAsync();
            return result.StatusCode switch
            {
                200 => Ok(result.Result),
                400 => BadRequest(result.Error),
                404 => NotFound(result.Error),
                _ => Problem(result.Error)
            };
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Returns a status by its ID.")]
    [SwaggerResponse(200, "Status received successfully.")]
    [SwaggerResponse(400, "Bad request.")]
    [SwaggerResponse(404, "Status not found.")]
    [SwaggerResponse(500, "An error occurred while retrieving the status.")]
    public async Task<IActionResult> GetStatusById(int id)
    {
        try
        {
            if(id <= 0)
                return BadRequest("Invalid ID provided");

            var result = await _bookingStatusService.GetByIdAsync(id);
            
            return result.StatusCode switch
            {
                200 => Ok(result.Result),
                400 => BadRequest(result.Error),
                404 => NotFound(result.Error),
                _ => Problem(result.Error)
            };
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }
}
