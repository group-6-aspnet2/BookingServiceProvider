using Data.Interfaces;
using Domain.Models;
using Domain.Responses;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Business.Services;

public interface IBookingStatusService
{
    Task<BookingStatusResult<IEnumerable<StatusModel>>> GetAllAsync();
    Task<BookingStatusResult<StatusModel>> GetByIdAsync(int id);
}

public class BookingStatusService(IBookingStatusRepository bookingStatusRepository, IMemoryCache cache) : IBookingStatusService
{

    private readonly IBookingStatusRepository _bookingStatusRepository = bookingStatusRepository;
    private readonly IMemoryCache _cache = cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);


    public async Task<BookingStatusResult<IEnumerable<StatusModel>>> GetAllAsync()
    {
        try
        {
            const string cacheKey = "StatusTypes";

            if(!_cache.TryGetValue(cacheKey, out IEnumerable<StatusModel>? statuses))
            {
                var result = await _bookingStatusRepository.GetAllAsync();
                if (!result.Succeeded)
                    return new BookingStatusResult<IEnumerable<StatusModel>> { Error = "Could not fetch booking statuses", StatusCode = 404, Succeeded = false };


                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheExpiration);
                _cache.Set(cacheKey, result.Result, cacheEntryOptions);

                return new BookingStatusResult<IEnumerable<StatusModel>>
                {
                    Result = result.Result,
                    StatusCode = 200,
                    Succeeded = true
                };
            }

            return new BookingStatusResult<IEnumerable<StatusModel>>
            {
                Result = statuses,
                StatusCode = 200,
                Succeeded = true
            };


        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingStatusResult<IEnumerable<StatusModel>> { Error = ex.Message, StatusCode = 500, Succeeded = false };
        }

    }
    public async Task<BookingStatusResult<StatusModel>> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
                return new BookingStatusResult<StatusModel>
                {
                    Error = "Invalid ID provided",
                    StatusCode = 400,
                    Succeeded = false
                };

            var result = await _bookingStatusRepository.GetAsync(x => x.Id == id);
            if (!result.Succeeded)
                return new BookingStatusResult<StatusModel> { Error = "Booking status not found", StatusCode = 404, Succeeded = false };

            return new BookingStatusResult<StatusModel>
            {
                Result = result.Result,
                StatusCode = 200,
                Succeeded = true
            };

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingStatusResult<StatusModel> { Error = ex.Message, StatusCode = 500, Succeeded = false };
        }
    }

}
