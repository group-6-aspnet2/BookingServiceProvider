using Domain.Models;
using Domain.Responses;

namespace Business.Interfaces;

public interface IBookingService
{
    Task<BookingResult> CancelBookingAsync(string id);
    Task<BookingResult<BookingModel>> CreateNewBookingAsync(CreateBookingForm form);
    Task<BookingResult> DeleteBookingWithTickets(string id);
    Task<BookingResult<IEnumerable<BookingModel>>> GetAllBookingsAsync();
    Task<BookingResult<IEnumerable<BookingModel>>> GetBookingsByEventIdAsync(string eventId);
    Task<BookingResult<IEnumerable<BookingModel>>> GetBookingsByStatusIdAsync(int statusId);
    Task<BookingResult<IEnumerable<BookingModel>>> GetBookingsByUserIdAsync(string userId);
    Task<BookingResult<BookingModel>> GetOneAsync(string id);
    Task<BookingResult<BookingModel>> UpdateBookingAsync(UpdateBookingForm form);
    //Task<BookingResult<BookingModel>> UpdateBookingWithInvoiceId(string bookingId, string invoiceId);
}