using BookingServiceProvider;
using BookingServiceProvider.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;


    // ADMIN
    [HttpGet]
    public async Task<IActionResult> GetBookings()
    {
        try
        {
            var result = await _bookingService.GetBookings(new GetBookingsRequest(), context: );
          
             return result.Succeeded 
                ? Ok(result.Bookings) 
                : BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }


    // ADMIN
    [HttpGet("eventId")]
    public async Task<IActionResult> GetBookingsByEvent(string eventId)
    {
        try
        {
            var request = new GetBookingsByEventIdRequest { EventId = eventId };
            var result = await _bookingService.GetBookingsByEventId(request, context: );

            return result.Succeeded
               ? Ok(result.Bookings)
               : BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }
}
