using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Data.Repositories;

public class BookingRepository(DataContext context) : BaseRepository<BookingEntity, BookingModel>(context), IBookingRepository
{


    public async Task<RepositoryResult<BookingModel>> GetAsyncWithStatus(Expression<Func<BookingEntity, bool>> findBy, params Expression<Func<BookingEntity, object>>[] includes)
    {

        try
        {
            IQueryable<BookingEntity> query = _table;

            if (findBy == null)
                return new RepositoryResult<BookingModel> { Succeeded = false, StatusCode = 400, Error = "Expression not defined." };

            if (includes != null && includes.Length != 0)
                foreach (var include in includes)
                    query = query.Include(include);

            var entity = await query.FirstOrDefaultAsync(findBy);

            if (entity == null)
                return new RepositoryResult<BookingModel> { Succeeded = false, StatusCode = 404, Error = "Entity not found." };


            var result = entity.MapTo<BookingModel>();
            result.StatusName = entity.Status.StatusName;

            return new RepositoryResult<BookingModel> { Succeeded = true, StatusCode = 200, Result = result };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult<BookingModel> { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    public async  Task<RepositoryResult<IEnumerable<BookingModel>>> GetAllAsyncWithStatus(bool orderByDescending = false, Expression<Func<BookingEntity, object>>? sortByColumn = null, Expression<Func<BookingEntity, bool>>? filterBy = null, int take = 0, params Expression<Func<BookingEntity, object>>[] includes)
    {
        try
        {

            IQueryable<BookingEntity> query = _table;

            if (filterBy != null)
                query = query.Where(filterBy);

            if (includes != null && includes.Length != 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (sortByColumn != null)
                query = orderByDescending
                    ? query.OrderByDescending(sortByColumn)
                    : query.OrderBy(sortByColumn);

            if (take > 0)
            {
                query = query.Take(take);
            }

            var entities = await query.ToListAsync();
          
          //  var result = entities.Select(entity => {
          //      var model = entity.MapTo<BookingModel>();
          //      model.StatusName = entity.Status.StatusName;
          //      return model;
          //      }
          //);
            var resultModels =new List<BookingModel>();


            for (int i = 0; i < entities.Count(); i++)
            {
                var model = entities[i].MapTo<BookingModel>();
                model.StatusName = entities[i].Status.StatusName;
                resultModels.Add(model);
            }



            return new RepositoryResult<IEnumerable<BookingModel>> { Succeeded = true, StatusCode = 200, Result = resultModels };
        }
        catch (Exception ex)
        {
            return new RepositoryResult<IEnumerable<BookingModel>> { Succeeded = false, StatusCode = 200, Error = ex.Message };

        }
    }


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
