using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingStatusesController(IBookingStatusService bookingStatusService) : ControllerBase
{
    private readonly IBookingStatusService _bookingStatusService = bookingStatusService;

    [HttpGet]
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
