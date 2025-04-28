using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Grpc.Core;
using System.Diagnostics;

namespace BookingServiceProvider.Services;

public interface IBookingService
{
    Task<CancelBookingReply> CancelBooking(CancelBookingRequest request, ServerCallContext context);
    Task<CreateBookingReply> CreateBooking(CreateBookingRequest request, ServerCallContext context);
    Task<GetBookingsReply> GetBookings(GetBookingsRequest request, ServerCallContext context);
    Task<GetBookingsByEventIdReply> GetBookingsByEventId(GetBookingsByEventIdRequest request, ServerCallContext context);
    Task<GetOneBookingReply> GetOneBooking(GetOneBookingRequest request, ServerCallContext context);
}

public class BookingService(IBookingRepository bookingRepository) : BookingManager.BookingManagerBase, IBookingService
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;

    public override async Task<GetBookingsReply> GetBookings(GetBookingsRequest request, ServerCallContext context)
    {
        try
        {
            // TODO kolla om user är admin


            var bookingResult = await _bookingRepository.GetAllAsync();
            if (bookingResult.Succeeded)
            {
                var bookings = bookingResult.Result?.Select(x => x.MapTo<Booking>());

                var reply = new GetBookingsReply
                {
                    Succeeded = true,
                    Bookings = { bookings }
                };
                return reply;
            }


            return new GetBookingsReply
            {
                Succeeded = false,
                Message = bookingResult.Error
            };

        }
        catch (Exception ex)
        {
            Debug.Write(ex.Message);
            return new GetBookingsReply
            {
                Succeeded = false,
                Message = ex.Message,
            };
        }
    }

    public override async Task<GetBookingsByEventIdReply> GetBookingsByEventId(GetBookingsByEventIdRequest request, ServerCallContext context)
    {
        try
        {
            if (request.EventId == null || string.IsNullOrWhiteSpace(request.EventId))
            {
                return new GetBookingsByEventIdReply
                {
                    Succeeded = false,
                    Message = "EventId is required",
                };
            }


            // TODO kolla om user är admin




            var bookingResult = await _bookingRepository.GetAllAsync(filterBy: x => x.EventId == request.EventId);
            if (bookingResult.Succeeded)
            {
                var bookings = bookingResult.Result?.Select(x => x.MapTo<Booking>());
                var reply = new GetBookingsByEventIdReply
                {
                    Succeeded = true,
                    Bookings = { bookings }
                };
            }

            return new GetBookingsByEventIdReply
            {
                Succeeded = false,
                Message = bookingResult.Error
            };
        }
        catch (Exception ex)
        {
            Debug.Write(ex.Message);
            return new GetBookingsByEventIdReply
            {
                Succeeded = false,
                Message = ex.Message,
            };
        }
    }

    public override async Task<GetOneBookingReply> GetOneBooking(GetOneBookingRequest request, ServerCallContext context)
    {
        try
        {
            if (request.BookingId == null || string.IsNullOrWhiteSpace(request.BookingId))
                return new GetOneBookingReply
                {
                    Succeeded = false,
                    Message = "BookingId is required",
                };



            // TODO kolla om user är admin ELLER om userId är samma som bokningens userId


            var bookingResult = await _bookingRepository.GetAsync(x => x.Id == request.BookingId);
            if (bookingResult.Succeeded)
            {
                var booking = bookingResult.Result?.MapTo<Booking>();
                var reply = new GetOneBookingReply
                {
                    Succeeded = true,
                    Booking = booking
                };
                return reply;
            }

            return new GetOneBookingReply
            {
                Succeeded = false,
                Message = $"Something went wrong when fetching booking with id {request.BookingId}",
            };
        }
        catch (Exception ex)
        {
            Debug.Write(ex.Message);
            return new GetOneBookingReply
            {
                Succeeded = false,
                Message = ex.Message,
            };
        }
    }


    public override Task<CreateBookingReply> CreateBooking(CreateBookingRequest request, ServerCallContext context)
    {
        return base.CreateBooking(request, context);
    }

    public override Task<CancelBookingReply> CancelBooking(CancelBookingRequest request, ServerCallContext context)
    {
        // TODO kolla om user är admin eller om userId är samma som bokningens userId 


        return base.CancelBooking(request, context);
    }

}
