using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;
using Domain.Responses;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories;

public class BookingRepository(DataContext context) : BaseRepository<BookingEntity, BookingModel>(context), IBookingRepository
{

    private readonly DataContext _context = context;



    public async Task<RepositoryResult> CancelBookingById(Expression<Func<BookingEntity, bool>> findBy, int statusId)
    {
		try
		{
			var bookingToCancel = _context.Bookings.FirstOrDefault(findBy);

			bookingToCancel!.StatusId = statusId;

            var result = _context.Update(bookingToCancel); // sätt status som cancelled
			await _context.SaveChangesAsync();

			return new RepositoryResult
			{
				Succeeded = true,
				StatusCode = 200 // TODO kolla om rätt vid update/delete statuscode 
			};

		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			return new RepositoryResult
			{
				Succeeded = false,
				StatusCode = 500,
				Error = ex.Message
			};

        }
    }
}
