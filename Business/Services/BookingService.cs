using Azure.Core;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;

namespace Business.Services;



public class BookingService(IBookingRepository bookingRepository, IBookingStatusRepository bookingStatusRepository, EventContract.EventContractClient eventClient) : IBookingService
{

    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IBookingStatusRepository _bookingStatusRepository = bookingStatusRepository;
    private readonly EventContract.EventContractClient _eventClient = eventClient;

    public async Task<BookingResult<IEnumerable<BookingModel>>> GetAllAsync()
    {
        try
        {
            var bookingResult = await _bookingRepository.GetAllAsync(orderByDescending: true, sortByColumn: x => x.CreateDate);

            if (bookingResult.Succeeded == false)
                return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, StatusCode = bookingResult.StatusCode, Error = bookingResult.Error };


            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = true, Result = bookingResult.Result, StatusCode = bookingResult.StatusCode };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, Error = ex.Message, StatusCode = 500 };
        }
    }


    public async Task<BookingResult<BookingModel>> GetOneAsync(string id)
    {
        try
        {
            var result = await _bookingRepository.GetAsync(x => x.Id == id);
            if (result.Succeeded == false)
                return new BookingResult<BookingModel> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };


            return new BookingResult<BookingModel> { Succeeded = true, StatusCode = result.StatusCode, Result = result.Result };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<BookingModel> { Succeeded = false, Error = ex.Message, StatusCode = 500 };
        }
    }



    public async Task<BookingResult<IEnumerable<BookingModel>>> GetBookingsByEventIdAsync(string eventId)
    {
        try
        {
            var bookingResult = await _bookingRepository.GetAllAsync(orderByDescending: true, sortByColumn: x => x.CreateDate, filterBy: x => x.EventId == eventId);

            if (bookingResult.Succeeded == false)
                return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, StatusCode = bookingResult.StatusCode, Error = bookingResult.Error };

            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = true, Result = bookingResult.Result, StatusCode = bookingResult.StatusCode };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, Error = ex.Message, StatusCode = 500 };
        }
    }

    public async Task<BookingResult<IEnumerable<BookingModel>>> GetBookingsByUserIdAsync(string userId)
    {
        try
        {
            var bookingResult = await _bookingRepository.GetAllAsync(orderByDescending: true, sortByColumn: x => x.CreateDate, filterBy: x => x.UserId == userId);

            if (bookingResult.Succeeded == false)
                return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, StatusCode = bookingResult.StatusCode, Error = bookingResult.Error };

            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = true, Result = bookingResult.Result, StatusCode = bookingResult.StatusCode };

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, Error = ex.Message, StatusCode = 500 };
        }
    }

    public async Task<BookingResult<IEnumerable<BookingModel>>> GetBookingsByStatusIdAsync(int statusId)
    {
        try
        {
            var bookingResult = await _bookingRepository.GetAllAsync(orderByDescending: true, sortByColumn: x => x.CreateDate, filterBy: x => x.StatusId == statusId);

            if (bookingResult.Succeeded == false)
                return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, StatusCode = bookingResult.StatusCode, Error = bookingResult.Error };

            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = true, Result = bookingResult.Result, StatusCode = bookingResult.StatusCode };

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, Error = ex.Message, StatusCode = 500 };
        }
    }


    public async Task<BookingResult<BookingModel>> CreateNewBookingAsync(CreateBookingForm form)
    {
        try
        {
            if (form == null)
                return new BookingResult<BookingModel> { Succeeded = false, Error = "Invalid new booking form" };

            var createRequest = form.MapTo<CreateBookingRequest>();
            createRequest.StatusId = 1;
            createRequest.TotalPrice = form.TicketQuantity * form.TicketPrice;


            var eventRequest = new GetEventByIdRequest { EventId = form.EventId };
            GetEventByIdReply eventReply = _eventClient.GetEventById(eventRequest);

            if (eventReply != null)
            {
                // TODO kolla events totalplatser och hämta alla bokningar för detta event. Kolla tillgänglighet. 
                var allEventBookingsResult = await GetBookingsByEventIdAsync(eventReply.Event.EventId);
                var totalBookingsToEvent = 0;

                if (allEventBookingsResult.Result != null)
                    foreach (var booking in allEventBookingsResult.Result)
                    {
                        totalBookingsToEvent += booking.TicketQuantity;
                    }


                // TODO kolla med cissi om amount of guests ska vara int istället i proto och db?
                if (int.Parse(eventReply.Event.EventAmountOfGuests) - totalBookingsToEvent < form.TicketQuantity)
                    return new BookingResult<BookingModel> { Succeeded = false, Error = "Not enough tickets available for this event" };
                
                createRequest.EventName = eventReply.Event.EventName;
                createRequest.EventCategoryName = eventReply.Event.EventCategoryName;
                createRequest.EventDate = DateOnly.Parse(eventReply.Event.EventDate);
            }
            else // endast som mockdata för att testa att skapa bokningar utan en fungerande eventsrevice. TODO ta bort sen
            {
                createRequest.EventName = "Green Day World Tour";
                createRequest.EventCategoryName = "Consert";
                createRequest.EventDate = DateOnly.FromDateTime(DateTime.Now);
            }

            //var userRequest = new GetUserByIdRequest
            //{
            //    UserId = form.UserId
            //};
            //var userReply = _userClient.GetUserById(userRequest)
            createRequest.FirstName = "Arvid";
            createRequest.LastName = "Vigren";
            createRequest.PhoneNumber = "0123456789";
            createRequest.Email = "arvid@domain.com";

            var entityToAdd = createRequest.MapTo<BookingEntity>();

            // TODO anropa azure service bus för att skapa evoucher och invoice, få tillbaka id:n.
            entityToAdd.EVoucherId = Guid.NewGuid().ToString();
            entityToAdd.InvoiceId = Guid.NewGuid().ToString();



            var result = await _bookingRepository.AddAsync(entityToAdd);

            return result.Succeeded
                ? new BookingResult<BookingModel> { Succeeded = true, StatusCode = result.StatusCode, Result = result.Result }
                : new BookingResult<BookingModel> { Succeeded = false, Error = result.Error, StatusCode = result.StatusCode };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<BookingModel> { Succeeded = false, Error = ex.Message, StatusCode = 500 };
        }
    }

    public async Task<BookingResult<BookingModel>> UpdateBookingAsync(UpdateBookingForm form)
    {
        try
        {
            var entityResult = await _bookingRepository.ExistsAsync(x => x.Id == form.Id);

            if (entityResult.Succeeded == false)
                return new BookingResult<BookingModel> { Succeeded = false, StatusCode = 404, Error = $"Could not find a booking with id {form.Id}." };

            var model = form.MapTo<BookingModel>();


            var allEventBookingsResult = await GetBookingsByEventIdAsync(form.EventId);
            var totalEventBookings = 0;

            if (allEventBookingsResult.Result != null)
                foreach (var booking in allEventBookingsResult.Result)
                {
                    if (booking.Id != form.Id)
                    {
                        totalEventBookings += booking.TicketQuantity;
                    }
                }


            /*
            if(event.totalAmountOfGuests - totalEventBookings < form.TicketQuantity)
                return new BookingResult<BookingModel> { Succeeded = false, Error = "Not enough tickets available for this event" };
            */

            model.TotalPrice = form.TicketPrice * form.TicketQuantity;
            var result = await _bookingRepository.UpdateBookingFromModelAsync(model);

            if (result.Succeeded == false)
                return new BookingResult<BookingModel> { Succeeded = false, StatusCode = 500, Error = "Something went wrong when updating the booking" };

            return new BookingResult<BookingModel> { Succeeded = true, StatusCode = result.StatusCode, Result = result.Result };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<BookingModel> { Succeeded = false, Error = ex.Message, StatusCode = 500 };
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

            var result = await _bookingRepository.UpdateBookingFromModelAsync(booking);

            return result.Succeeded
                ? new BookingResult { Succeeded = true, StatusCode = 204 }
                : new BookingResult { Succeeded = false, StatusCode = 500, Error = "Something went wrong when canceling the booking" };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult { Succeeded = false, Error = ex.Message, StatusCode = 500 };
        }
    }

}
