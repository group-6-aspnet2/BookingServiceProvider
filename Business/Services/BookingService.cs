using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;
using System.Diagnostics;

namespace Business.Services;



public class BookingService(IBookingRepository bookingRepository, IBookingStatusRepository bookingStatusRepository) : IBookingService
{

    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IBookingStatusRepository _bookingStatusRepository = bookingStatusRepository;

    public async Task<BookingResult<IEnumerable<BookingModel>>> GetAllAsync()
    {
        try
        {
            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = true };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, Error = ex.Message };
        }
    }


    public async Task<BookingResult<BookingModel>> GetOneAsync(string id)
    {
        try
        {
            return new BookingResult<BookingModel> { Succeeded = true };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<BookingModel> { Succeeded = false, Error = ex.Message };
        }
    }



    public async Task<BookingResult<IEnumerable<BookingModel>>> GetBookingsByEventIdAsync(string eventId)
    {
        try
        {
            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = true };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, Error = ex.Message };
        }
    }

    public async Task<BookingResult<IEnumerable<BookingModel>>> GetBookingsByUserIdAsync(string userId)
    {
        try
        {
            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = true };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, Error = ex.Message };
        }
    }

    public async Task<BookingResult<BookingModel>> CreateNewBookingAsync(CreateBookingForm form)
    {
        try
        {
            if (form == null) 
                return new BookingResult<BookingModel> { Succeeded = false, Error = "Invalid new booking form" };

            var createRequest = form.MapTo<CreateBookingRequest>();

            //Mockdata för testning, istället för data som hämtas via grpc från UserProver och EventProvider
            createRequest.EventName = "Parkteater";
            createRequest.EventCategoryId = 2;
            createRequest.Date = DateTime.Now;
            createRequest.FirstName = "Pelle";
            createRequest.LastName = "Svanslös";
            createRequest.PhoneNumber = "0123456789";
            createRequest.Email = "pelle@domain.com";

            var entityToAdd = createRequest.MapTo<BookingEntity>();
            entityToAdd.EVoucherId = Guid.NewGuid().ToString();
            entityToAdd.InvoiceId = Guid.NewGuid().ToString();



            var result = await _bookingRepository.AddAsync(entityToAdd);

            return result.Succeeded 
                ?  new BookingResult<BookingModel> { Succeeded = true, StatusCode = result.StatusCode, Result = result.Result }
                : new BookingResult<BookingModel> { Succeeded = false, Error = result.Error,StatusCode = result.StatusCode };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<BookingModel> { Succeeded = false, Error = ex.Message };
        }
    }

    public async Task<BookingResult<BookingModel>> UpdateBookingAsync(UpdateBookingForm form)
    {
        try
        {

            return new BookingResult<BookingModel> { Succeeded = true };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<BookingModel> { Succeeded = false, Error = ex.Message };
        }
    }

    public async Task<BookingResult> CancelBookingAsync(string id)
    {
        try
        {
            var bookingResult = await _bookingRepository.GetAsync(x => x.Id == id);

            if (bookingResult.Succeeded == false || bookingResult.Result == null)
                return new BookingResult { Succeeded = false, StatusCode = 404, Error = $"Could not find a booking with id {id}." };

            var booking = bookingResult.Result;
            var statusResult = await _bookingStatusRepository.GetAllAsync();

            if (statusResult.Succeeded == false)
                return new BookingResult { Succeeded = false, StatusCode = 404, Error = $"Could not find any statuses" };

            var cancelledStatus = statusResult.Result?.FirstOrDefault(x => x.StatusName == "Cancelled");

            if (cancelledStatus != null)
                booking.StatusId = cancelledStatus.Id;

            var bookingEntityToUpdateStatus = booking.MapTo<BookingEntity>();
            var result = await _bookingRepository.UpdateAsync(bookingEntityToUpdateStatus);

            return result.Succeeded
                ? new BookingResult { Succeeded = true, StatusCode = 200 }
                : new BookingResult { Succeeded = false, StatusCode = 500, Error = "Something went wrong when cancelling the booking" };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult { Succeeded = false, Error = ex.Message };
        }
    }

}
