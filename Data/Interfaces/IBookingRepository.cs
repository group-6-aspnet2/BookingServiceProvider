using Data.Entities;
using Domain.Models;
using Domain.Responses;
using System.Linq.Expressions;

namespace Data.Interfaces;

public interface IBookingRepository : IBaseRepository<BookingEntity, BookingModel>
{
    Task<BookingResult<BookingModel>> UpdateBookingFromModelAsync(BookingModel model);
}
