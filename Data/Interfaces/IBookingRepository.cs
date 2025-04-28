using Data.Entities;
using Domain.Models;

namespace Data.Interfaces;

public interface IBookingRepository : IBaseRepository<BookingEntity, BookingModel>
{
}