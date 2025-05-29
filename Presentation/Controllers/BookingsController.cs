using Business.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Documentation;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Controllers;


[Produces("application/json")]
[Consumes("application/json")]
[Route("api/[controller]")]
[ApiController]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;
   
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Returns a booking by its ID.")]
    [SwaggerResponse(200, "Booking received successfully.")]
    [SwaggerResponse(404, "Booking not found.")]
    [SwaggerResponse(500, "An error occurred while retrieving the booking.")]
    [SwaggerResponseExample(200,typeof(Booking_Example))]
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


    [HttpGet]
    [SwaggerOperation(Summary = "Returns a list of bookings")]
    [SwaggerResponse(200, "Bookings recieved successfully.")]
    [SwaggerResponse(500, "Bookings could not be recieved.")]
    [SwaggerResponseExample(200, typeof(Bookings_Example))]
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
   
    [HttpGet("status/{statusId}")]
    [SwaggerOperation(Summary = "Returns a list of bookings with the specified status ID")]
    [SwaggerResponse(200, "Bookings with the specified status ID received successfully.")]
    [SwaggerResponse(404, "No status with the provided ID exists or no bookings found with the specified status ID.")]
    [SwaggerResponse(500, "An error occurred while retrieving bookings by status ID.")]
    [SwaggerResponseExample(200, typeof(BookingsByStatus_Example))]

    public async Task<IActionResult> GetBookingsByStatus(int statusId)
    {
        try
        {
            var result = await _bookingService.GetBookingsByStatusIdAsync(statusId);

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


    [HttpGet("event/{eventId}")]
    [SwaggerOperation(Summary = "Returns a list of bookings for a specific event by its ID.")]
    [SwaggerResponse(200, "Bookings for the specified event received successfully.")]
    [SwaggerResponse(404, "No event with the provided ID exists or no bookings found for the specified event.")]
    [SwaggerResponse(500, "An error occurred while retrieving bookings by event ID.")]
    [SwaggerResponseExample(200, typeof(BookingsByEvent_Example))]
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

    [HttpGet("user/{userId}")]
    [SwaggerOperation(Summary = "Returns a list of bookings for a specific user by their ID.")]
    [SwaggerResponse(200, "Bookings for the specified user received successfully.")]
    [SwaggerResponse(404, "No user with the provided ID exists or no bookings found for the specified user.")]
    [SwaggerResponse(500, "An error occurred while retrieving bookings by user ID.")]
    [SwaggerResponseExample(200, typeof(BookingsByUser_Example))]
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
    [SwaggerOperation(Summary = "Adds a booking to the list of bookings.")]
    [SwaggerResponse(201, "Booking added successfully.")]
    [SwaggerResponse(400, "Booking request contained invalid or missing properties.")]
    [SwaggerResponse(404, "Event or user not found.")]
    [SwaggerResponse(500, "An error occurred while adding the booking.")]
    [SwaggerRequestExample(typeof(CreateBookingForm), typeof(CreateBookingForm_Example))]
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
    [SwaggerOperation(Summary = "Updates an existing booking.")]
    [SwaggerResponse(200, "Booking updated successfully.")]
    [SwaggerResponse(400, "Booking request contained invalid or missing properties.")]
    [SwaggerResponse(404, "Booking not found.")]
    [SwaggerResponse(500, "An error occurred while updating the booking.")]
    [SwaggerRequestExample(typeof(UpdateBookingForm), typeof(UpdateBookingForm_Example))]
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
    [SwaggerOperation(Summary = "Cancels a booking by its ID.")]
    [SwaggerResponse(204, "Booking cancelled successfully but is still in the list of bookings.")]
    [SwaggerResponse(400, "Booking cancellation request contained invalid or missing properties.")]
    [SwaggerResponse(404, "Booking not found.")]
    [SwaggerResponse(500, "An error occurred while cancelling the booking.")]
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
    [SwaggerOperation(Summary = "Deletes a booking by booking ID.")]
    [SwaggerResponse(204, "Booking deleted successfully.")]
    [SwaggerResponse(400, "Booking deletion request contained invalid or missing properties.")]
    [SwaggerResponse(404, "Booking not found.")]
    [SwaggerResponse(500, "An error occurred while deleting the booking.")]
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
