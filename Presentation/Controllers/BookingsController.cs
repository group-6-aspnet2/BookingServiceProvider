using Business.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;

    // ADMIN och USER som står på bokningen
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById(string id)
    {
        try
        {
            var result = await _bookingService.GetOneAsync(id);

            return result.StatusCode switch
            {
                200 => Ok(result.Result),
                404 => NotFound(result.Error),
                _ => Problem(result.Error)
            };
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }


    // ADMIN
    [HttpGet]
    public async Task<IActionResult> GetBookings()
    {
        try
        {
            var result = await _bookingService.GetAllAsync();

            return result.StatusCode switch
            {
                200 => Ok(result.Result),
                _ => Problem(result.Error)
            };
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }

    // ADMIN
    [HttpGet("status/{statusId}")]
    public async Task<IActionResult> GetBookings(int statusId)
    {
        try
        {
            var result = await _bookingService.GetBookingsByStatusIdAsync(statusId);

            return result.StatusCode switch
            {
                200 => Ok(result.Result),
                _ => Problem(result.Error)
            };
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }


    // ADMIN
    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetBookingsByEvent(string eventId)
    {
        try
        {
            var result = await _bookingService.GetBookingsByEventIdAsync(eventId );

            return result.StatusCode switch
            {
                200 => Ok(result.Result),
                404 => NotFound(result.Error),
                _ => Problem(result.Error)
            };
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }

    // ADMIN
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetBookingsByUser(string userId)
    {
        try
        {
            var result = await _bookingService.GetBookingsByUserIdAsync(userId);

            return result.StatusCode switch
            {
                200 => Ok(result.Result),
                404 => NotFound(result.Error),
                _ => Problem(result.Error)
            };
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }


    [HttpPost]
    public async Task<IActionResult> AddNewBooking(CreateBookingForm form)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid booking form" });

            var createResult = await _bookingService.CreateNewBookingAsync(form);
            return createResult.StatusCode switch
            {
                201 => Created("Booking was successfully created!", createResult.Result),
                400 => BadRequest(createResult.Error),
                404 => NotFound(createResult.Error),
                _ => Problem(createResult.Error),
            };
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }


    //[HttpPut]
    //public async Task<IActionResult> UpdateBooking(UpdateBookingForm form)
    //{
    //    try
    //    {
    //        if (!ModelState.IsValid)
    //            return BadRequest(new { message = "Invalid booking form" });

    //        var createResult = await _bookingService.CreateNewBookingAsync(form);
    //        return createResult.StatusCode switch
    //        {
    //            201 => Created("Booking was successfully created!", createResult.Result),
    //            400 => BadRequest(createResult.Error),
    //            404 => NotFound(createResult.Error),
    //            _ => Problem(createResult.Error),
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        return Problem(detail: ex.Message);
    //    }
    //}


    //[HttpDelete]
    //public async Task<IActionResult> CancelBooking(CreateBookingForm form)
    //{
    //    try
    //    {
    //        if (!ModelState.IsValid)
    //            return BadRequest(new { message = "Invalid booking form" });

    //        var createResult = await _bookingService.CreateNewBookingAsync(form);
    //        return createResult.StatusCode switch
    //        {
    //            201 => Created("Booking was successfully created!", createResult.Result),
    //            400 => BadRequest(createResult.Error),
    //            404 => NotFound(createResult.Error),
    //            _ => Problem(createResult.Error),
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        return Problem(detail: ex.Message);
    //    }
    //}
}
