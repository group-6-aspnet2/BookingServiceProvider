using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories;

public class BookingRepository(DataContext context) : BaseRepository<BookingEntity, BookingModel>(context), IBookingRepository
{

    public async Task<BookingResult<BookingModel>> UpdateBookingFromModelAsync(BookingModel model)
    {
        try
        {
            var bookingEntity = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (bookingEntity == null)
                return new BookingResult<BookingModel> { Succeeded = false, StatusCode = 404, Error = "Booking entity was not found." };

            bookingEntity.StatusId = model.StatusId;
            bookingEntity.EventId = model.EventId;
            bookingEntity.InvoiceId = model.InvoiceId;
            bookingEntity.TicketCategoryName = model.TicketCategoryName;
            bookingEntity.TicketPrice = model.TicketPrice;
            bookingEntity.TicketQuantity = model.TicketQuantity;
            bookingEntity.UserId = model.UserId;

            _table.Update(bookingEntity);
            await _context.SaveChangesAsync();
            var updatedModel = bookingEntity.MapTo<BookingModel>();

            return new BookingResult<BookingModel> { Succeeded = true, StatusCode = 200, Result = updatedModel };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<BookingModel> { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }
}
