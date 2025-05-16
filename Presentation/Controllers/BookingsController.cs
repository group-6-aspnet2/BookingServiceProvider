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
            var result = await _bookingService.GetAllBookingsAsync();

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
    public async Task<IActionResult> GetBookingsByStatus(int statusId)
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


    [HttpPut]
    public async Task<IActionResult> UpdateBooking(UpdateBookingForm form)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid booking form" });

            var updateResult = await _bookingService.UpdateBookingAsync(form);
            return updateResult.StatusCode switch
            {
                200 => Ok( updateResult.Result),
                400 => BadRequest(updateResult.Error),
                404 => NotFound(updateResult.Error),
                _ => Problem(updateResult.Error),
            };
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelBooking(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { message = "No booking id provided" });

            var cancelResult = await _bookingService.CancelBookingAsync(id);
            return cancelResult.StatusCode switch
            {
                204 => NoContent(),
                400 => BadRequest(cancelResult.Error),
                404 => NotFound(cancelResult.Error),
                _ => Problem(cancelResult.Error),
            };
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }

    [HttpDelete("delete/{id}")] 
    public async Task<IActionResult> DeleteBooking(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { message = "No booking id provided" });

            var deleteResult = await _bookingService.DeleteBookingWithTickets(id);
            return deleteResult.StatusCode switch
            {
                204 => NoContent(),
                400 => BadRequest(deleteResult.Error),
                404 => NotFound(deleteResult.Error),
                _ => Problem(deleteResult.Error),
            };
            
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message);
        }
    }
}
