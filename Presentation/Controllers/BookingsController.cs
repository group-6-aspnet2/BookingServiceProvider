using Business.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;


    // ADMIN
    //[HttpGet]
    //public async Task<IActionResult> GetBookings()
    //{
    //    try
    //    {
    //        var result = await _bookingService.GetBookings(new GetBookingsRequest(), context:  ); // TODO fråga hans om context andra parameter?

    //         return result.Succeeded 
    //            ? Ok(result.Bookings) 
    //            : BadRequest(result.Message);
    //    }
    //    catch (Exception ex)
    //    {
    //        return Problem(detail: ex.Message, statusCode: 500);
    //    }
    //}


    // ADMIN
    //[HttpGet("eventId")]
    //public async Task<IActionResult> GetBookingsByEvent(string eventId)
    //{
    //    try
    //    {
    //        var request = new GetBookingsByEventIdRequest { EventId = eventId };
    //        var result = await _bookingService.GetBookingsByEventId(request, context: );

    //        return result.Succeeded
    //           ? Ok(result.Bookings)
    //           : BadRequest(result.Message);
    //    }
    //    catch (Exception ex)
    //    {
    //        return Problem(detail: ex.Message, statusCode: 500);
    //    }
    //}


    [HttpPost]
    public async Task<IActionResult> AddNewBooking(CreateBookingForm form)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid booking form" });

            var createResult = await _bookingService.CreateNewBookingAsync(form);
            return createResult.Succeeded 
                ? Ok() 
                : Problem(createResult.Error);

        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }
}
