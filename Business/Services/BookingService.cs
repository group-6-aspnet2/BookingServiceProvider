using Business.Factories;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;
using System.Diagnostics;
using System.Text.Json;

namespace Business.Services;



public class BookingService(IBookingRepository bookingRepository, IBookingStatusRepository bookingStatusRepository, EventContract.EventContractClient eventClient, IInvoiceServiceBusHandler invoiceServiceBus, ITicketServiceBusHandler ticketServiceBusHandler, IEmailRestService emailService, IAccountRestService accountRestService) : IBookingService
{

    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IBookingStatusRepository _bookingStatusRepository = bookingStatusRepository;
    private readonly EventContract.EventContractClient _eventClient = eventClient;
    //private readonly AccountGrpcService.AccountGrpcServiceClient _accountGrpcServiceClient = accountGrpcServiceClient;
    private readonly IInvoiceServiceBusHandler _invoiceServiceBus = invoiceServiceBus;
    private readonly ITicketServiceBusHandler _ticketServiceBusHandler = ticketServiceBusHandler;
    private readonly IEmailRestService _emailService = emailService;
    private readonly IAccountRestService _accountRestService = accountRestService;

    public async Task<BookingResult<BookingModel>> GetOneAsync(string id)
    {
        try
        {
            var result = await _bookingRepository.GetAsyncWithStatus(findBy: x => x.Id == id, includes: x => x.Status);
            if (result.Succeeded == false || result.Result == null)
                return new BookingResult<BookingModel> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

            var booking = result.Result;

            var eventResult = await _eventClient.GetEventByIdAsync(new GetEventByIdRequest { EventId = booking.EventId });
            var bookingWithEvent = BookingFactory.MapEventToBookingModel(booking, eventResult.Event);

            //var accountRequest = new GetAccountByIdRequest { UserId = booking.UserId };
            //var accountResult = _accountGrpcServiceClient.GetAccountById(accountRequest);
            //if (!accountResult.Succeeded || accountResult.Account == null)
            //    return new BookingResult<BookingModel> { StatusCode = 404, Succeeded = false, Error = "Could not find account with provided Id" };


            //var bookingWithAllData = BookingFactory.MapUserToBookingModel(bookingWithEvent!, accountResult.Account);
            return new BookingResult<BookingModel> { Succeeded = true, StatusCode = result.StatusCode, Result = bookingWithEvent };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<BookingModel> { Succeeded = false, Error = ex.Message, StatusCode = 500 };
        }
    }

    public async Task<BookingResult<IEnumerable<BookingModel>>> GetAllBookingsAsync()
    {
        try
        {
            var bookingResult = await _bookingRepository.GetAllAsyncWithStatus(orderByDescending: true, sortByColumn: x => x.CreateDate, includes: x => x.Status);
            if (bookingResult.Succeeded == false)
                return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, StatusCode = bookingResult.StatusCode, Error = bookingResult.Error };

            var bookingsWithEventAndUserData = new List<BookingModel>();

            foreach (var booking in bookingResult.Result!)
            {
                var eventResult = await _eventClient.GetEventByIdAsync(new GetEventByIdRequest { EventId = booking.EventId });
                var bookingWithEvent = BookingFactory.MapEventToBookingModel(booking, eventResult.Event);

                //var accountRequest = new GetAccountByIdRequest { UserId = booking.UserId };
                //var accountResult = _accountGrpcServiceClient.GetAccountById(accountRequest);
                //if (!accountResult.Succeeded || accountResult.Account == null)
                //    return new BookingResult<IEnumerable<BookingModel>> { StatusCode = 404, Succeeded = false, Error = "Could not find account with provided Id" };

                //var bookingWithAllData = BookingFactory.MapUserToBookingModel(bookingWithEvent!, accountResult.Account);
                bookingsWithEventAndUserData.Add(bookingWithEvent!);
            }

            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = true, Result = bookingsWithEventAndUserData, StatusCode = bookingResult.StatusCode };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, Error = ex.Message, StatusCode = 500 };
        }
    }

    public async Task<BookingResult<IEnumerable<BookingModel>>> GetBookingsByEventIdAsync(string eventId)
    {
        try
        {
            var bookingResult = await _bookingRepository.GetAllAsyncWithStatus(orderByDescending: true, sortByColumn: x => x.CreateDate, filterBy: x => x.EventId == eventId, includes: x => x.Status);

            if (bookingResult.Succeeded == false)
                return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, StatusCode = bookingResult.StatusCode, Error = bookingResult.Error };

            var bookingsWithEventAndUserData = new List<BookingModel>();

            foreach (var booking in bookingResult.Result!)
            {
                var eventResult = await _eventClient.GetEventByIdAsync(new GetEventByIdRequest { EventId = booking.EventId });
                var bookingWithEvent = BookingFactory.MapEventToBookingModel(booking, eventResult.Event);

                //var accountRequest = new GetAccountByIdRequest { UserId = booking.UserId };
                //var accountResult = _accountGrpcServiceClient.GetAccountById(accountRequest);
                //if (!accountResult.Succeeded || accountResult.Account == null)
                //    return new BookingResult<IEnumerable<BookingModel>> { StatusCode = 404, Succeeded = false, Error = "Could not find account with provided Id" };


                //var bookingWithAllData = BookingFactory.MapUserToBookingModel(bookingWithEvent!, accountResult.Account);
                bookingsWithEventAndUserData.Add(bookingWithEvent!);
            }

            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = true, Result = bookingsWithEventAndUserData, StatusCode = bookingResult.StatusCode };
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
            var bookingResult = await _bookingRepository.GetAllAsyncWithStatus(orderByDescending: true, sortByColumn: x => x.CreateDate, filterBy: x => x.UserId == userId, includes: x => x.Status);

            if (bookingResult.Succeeded == false)
                return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, StatusCode = bookingResult.StatusCode, Error = bookingResult.Error };

            var bookingsWithEventAndUserData = new List<BookingModel>();

            foreach (var booking in bookingResult.Result!)
            {
                var eventResult = await _eventClient.GetEventByIdAsync(new GetEventByIdRequest { EventId = booking.EventId });
                var bookingWithEvent = BookingFactory.MapEventToBookingModel(booking, eventResult.Event);

                //var accountRequest = new GetAccountByIdRequest { UserId = booking.UserId };
                //var accountResult = _accountGrpcServiceClient.GetAccountById(accountRequest);
                //if (!accountResult.Succeeded || accountResult.Account == null)
                //    return new BookingResult<IEnumerable<BookingModel>> { StatusCode = 404, Succeeded = false, Error = "Could not find account with provided Id" };


                //var bookingWithAllData = BookingFactory.MapUserToBookingModel(bookingWithEvent!, accountResult.Account);
                //bookingsWithEventAndUserData.Add(bookingWithAllData!);
            }

            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = true, Result = bookingsWithEventAndUserData, StatusCode = bookingResult.StatusCode };

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
            var bookingResult = await _bookingRepository.GetAllAsyncWithStatus(orderByDescending: true, sortByColumn: x => x.CreateDate, filterBy: x => x.StatusId == statusId, includes: x => x.Status);

            if (bookingResult.Succeeded == false)
                return new BookingResult<IEnumerable<BookingModel>> { Succeeded = false, StatusCode = bookingResult.StatusCode, Error = bookingResult.Error };

            var bookingsWithEventAndUserData = new List<BookingModel>();

            foreach (var booking in bookingResult.Result!)
            {
                var eventResult = await _eventClient.GetEventByIdAsync(new GetEventByIdRequest { EventId = booking.EventId });
                var bookingWithEvent = BookingFactory.MapEventToBookingModel(booking, eventResult.Event);

                //var accountRequest = new GetAccountByIdRequest { UserId = booking.UserId };
                //var accountResult = _accountGrpcServiceClient.GetAccountById(accountRequest);

                //if (!accountResult.Succeeded || accountResult.Account == null)
                //    return new BookingResult<IEnumerable<BookingModel>> { StatusCode = 404, Succeeded = false, Error = "Could not find account with provided Id" };

                //var bookingWithAllData = BookingFactory.MapUserToBookingModel(bookingWithEvent!, accountResult.Account);
                bookingsWithEventAndUserData.Add(bookingWithEvent!);
            }

            return new BookingResult<IEnumerable<BookingModel>> { Succeeded = true, Result = bookingsWithEventAndUserData, StatusCode = bookingResult.StatusCode };

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

            var createRequest = form.MapTo<CreateBookingRequestModel>();
            createRequest.StatusId = 1;

            var eventRequest = new GetEventByIdRequest { EventId = form.EventId };
            GetEventByIdReply eventReply = await _eventClient.GetEventByIdAsync(eventRequest);

            if (eventReply != null)
            {
                var allEventBookingsResult = await GetBookingsByEventIdAsync(eventReply.Event.EventId);
                var totalBookingsToEvent = 0;

                if (allEventBookingsResult.Result != null)
                    foreach (var booking in allEventBookingsResult.Result)
                    {
                        totalBookingsToEvent += booking.TicketQuantity;
                    }

                if (eventReply.Event.EventAmountOfGuests - totalBookingsToEvent < form.TicketQuantity)
                    return new BookingResult<BookingModel> { Succeeded = false, Error = "Not enough tickets available for this event" };
            }

            var accountResult = await _accountRestService.GetAccountByIdAsync(form.UserId);

            if (accountResult == null)
                return new BookingResult<BookingModel> { Succeeded = false, StatusCode = 404, Error = "Account with provided ID could not be found." };


            var entityToAdd = createRequest.MapTo<BookingEntity>();
            var result = await _bookingRepository.AddAsync(entityToAdd);

            var inovicePayload = JsonSerializer.Serialize(new CreateInvoicePayload
            {
                BookingId = entityToAdd.Id,
                UserId = form.UserId,
                EventId = form.EventId,
                TicketQuantity = form.TicketQuantity,
                TicketPrice = form.TicketPrice,
                TicketCategoryName = form.TicketCategoryName

            });

            var ticketPayload = JsonSerializer.Serialize(new CreateTicketPayload
            {
                BookingId = entityToAdd.Id,
                UserId = form.UserId,
                EventId = form.EventId,
                TicketQuantity = form.TicketQuantity,
                TicketPrice = form.TicketPrice,
                TicketCategoryName = form.TicketCategoryName
            });


            var emailPayload = new BookingConfirmationRequest
            {
                BookingId = entityToAdd.Id,
                UserId = form.UserId,
                EventId = form.EventId,
                UserEmail = accountResult.Email
            };

            if (result.Succeeded)
            {
                await _invoiceServiceBus.PublishAsync(inovicePayload);
                await _ticketServiceBusHandler.PublishAsync(ticketPayload);
                await _emailService.SendBookingConfirmationAsync(emailPayload);


                var bookingWithEvent = result.Result!.MapTo<BookingModel>();
                bookingWithEvent = BookingFactory.MapEventToBookingModel(bookingWithEvent, eventReply!.Event);
                var bookingWithAllData = BookingFactory.MapAccountModelToBookingModel(bookingWithEvent!, accountResult);


                return new BookingResult<BookingModel> { Succeeded = true, StatusCode = result.StatusCode, Result = bookingWithEvent };
            }
            return new BookingResult<BookingModel> { Succeeded = false, Error = result.Error, StatusCode = result.StatusCode };
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

            var eventRequest = new GetEventByIdRequest { EventId = form.EventId };
            GetEventByIdReply eventReply = await _eventClient.GetEventByIdAsync(eventRequest);

            if (eventReply.Event.EventAmountOfGuests - totalEventBookings < form.TicketQuantity)
                return new BookingResult<BookingModel> { Succeeded = false, Error = "Not enough tickets available for this event" };


            //var accountRequest = new GetAccountByIdRequest { UserId = form.UserId };
            //var accountResult = _accountGrpcServiceClient.GetAccountById(accountRequest);

            //if (!accountResult.Succeeded || accountResult.Account == null)
            //    return new BookingResult<BookingModel> { Succeeded = false, StatusCode = 404, Error = "Account with provided ID could not be found." };

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

    public async Task<BookingResult> DeleteBookingWithTickets(string id)
    {
        try
        {
            var bookingResult = await _bookingRepository.GetAsync(x => x.Id == id);

            if (bookingResult.Succeeded == false || bookingResult.Result == null)
                return new BookingResult { Succeeded = false, StatusCode = 404, Error = $"Could not find a booking with id {id}." };

            var result = await _bookingRepository.DeleteAsync(x => x.Id == id);
            // TODO radera tickets, anrop antingen via grpc och vänta in, eller kör service bus

            if (result.Succeeded)
            {
                // TODO säg till service bus queue som ticketServiceProvider lyssnar på att köra DeleteTicketsByBookingId(bookingId).
                return new BookingResult { Succeeded = true, StatusCode = 204 };
            }
            ;

            return new BookingResult { Succeeded = false, StatusCode = 500, Error = "Something went wrong when deleting the booking" };

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult { Succeeded = false, Error = ex.Message, StatusCode = 500 };
        }
    }

    public async Task<BookingResult<BookingModel>> UpdateBookingInvoiceIdAsync(UpdateBookingInvoiceIdForm form)
    {
        try
        {
            if (form == null || string.IsNullOrWhiteSpace(form.BookingId) || string.IsNullOrWhiteSpace(form.InvoiceId))
                return new BookingResult<BookingModel> { Succeeded = false, Error = "Invalid ids provided" };

            var booking = await _bookingRepository.GetAsync(x => x.Id == form.BookingId);

            if (booking.Succeeded == false || booking.Result == null)
                return new BookingResult<BookingModel> { Succeeded = false, Error = "Could not find a booking with the provided id" };

            var updatedModel = booking.Result.MapTo<BookingModel>();
            updatedModel.InvoiceId = form.InvoiceId;

            var result = await _bookingRepository.UpdateBookingFromModelAsync(updatedModel);

            return result.Succeeded
                ? new BookingResult<BookingModel> { Succeeded = true, StatusCode = 204, Result = updatedModel }
                : new BookingResult<BookingModel> { Succeeded = false, StatusCode = 500, Error = "Something went wrong when updating the booking" };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<BookingModel> { Succeeded = false, Error = ex.Message, StatusCode = 500 };
        }
    }

}
