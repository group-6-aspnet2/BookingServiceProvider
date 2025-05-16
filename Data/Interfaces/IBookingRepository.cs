using Data.Entities;
using Domain.Models;
using Domain.Responses;
using System.Linq.Expressions;

namespace Data.Interfaces;

public interface IBookingRepository : IBaseRepository<BookingEntity, BookingModel>
{
    Task<RepositoryResult<IEnumerable<BookingModel>>> GetAllAsyncWithStatus(bool orderByDescending = false, Expression<Func<BookingEntity, object>>? sortByColumn = null, Expression<Func<BookingEntity, bool>>? filterBy = null, int take = 0, params Expression<Func<BookingEntity, object>>[] includes);
    Task<RepositoryResult<BookingModel>> GetAsyncWithStatus(Expression<Func<BookingEntity, bool>> findBy, params Expression<Func<BookingEntity, object>>[] includes);
    Task<BookingResult<BookingModel>> UpdateBookingFromModelAsync(BookingModel model);
}
