using Business.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Google.Protobuf.WellKnownTypes;
using Google.Type;
using Grpc.Core;
using System;
using System.Diagnostics;

namespace Presentation.GrpcServices;


public interface IBookingGrpcService
{
    Task<CancelBookingReply> CancelBooking(CancelBookingRequest request, ServerCallContext context);
    Task<CreateBookingReply> CreateBooking(CreateBookingRequest request, ServerCallContext context);
    Task<GetBookingsReply> GetBookings(GetBookingsRequest request, ServerCallContext context);
    Task<GetBookingsByEventIdReply> GetBookingsByEventId(GetBookingsByEventIdRequest request, ServerCallContext context);
    Task<GetOneBookingReply> GetOneBooking(GetOneBookingRequest request, ServerCallContext context);
}



public class BookingGrpcService(IBookingService bookingService) : BookingManager.BookingManagerBase, IBookingGrpcService
{

    private readonly IBookingService _bookingService = bookingService;
    public override async Task<GetBookingsReply> GetBookings(GetBookingsRequest request, ServerCallContext context)
    {
        try
        {
            var bookingResult = await _bookingService.GetAllBookingsAsync();
            if (bookingResult.Succeeded)
            {
                //var bookings = bookingResult.Result?.Select(x => x.MapTo<Booking>());

                List<Booking> bookings = [];
                foreach (var x in bookingResult.Result!)
                {
                    bookings.Add(
                        new Booking
                        {
                            Id = x.Id,
                            EventId = x.EventId,
                            StatusId = x.StatusId,
                            InvoiceId = x.InvoiceId,
                            UserId = x.UserId,
                            TicketCategoryName = x.TicketCategoryName,
                            TicketPrice = x.TicketPrice.ToString(),
                            TicketQuantity = x.TicketQuantity,
                            CreateDate = Timestamp.FromDateTime(x.CreateDate.ToUniversalTime()),
                        }
                    );
                }

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

            var bookingResult = await _bookingService.GetBookingsByEventIdAsync(request.EventId);
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


            var bookingResult = await _bookingService.GetOneAsync(request.BookingId);
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

    
  public override async Task<CreateBookingReply> CreateBooking(CreateBookingRequest request, ServerCallContext context)
  {
      try
      {
          if (request == null)
          {
              return new CreateBookingReply
              {
                  Succeeded = false,
                  Message = "Invalid create request"
              };
          }

          
            var form  = new CreateBookingForm
          {
              EventId = request.EventId,
              UserId = request.UserId,
              TicketCategoryName = request.TicketCategoryName,
              TicketPrice = decimal.Parse(request.TicketPrice),
              TicketQuantity = request.TicketQuantity,
            };
            var createResult = await _bookingService.CreateNewBookingAsync(form);
            var booking = createResult.Result?.MapTo<Booking>();
            return createResult.Succeeded
              ? new CreateBookingReply { Succeeded = true, Booking = booking }
              : new CreateBookingReply { Succeeded = false, Message = "Something went wrong when creating the booking" };

      }
      catch (Exception ex)
      {
          Debug.Write(ex.Message);
          return new CreateBookingReply
          {
              Succeeded = false,
              Message = ex.Message,
          };
      }
  }


    /*
  public override async Task<CancelBookingReply> CancelBooking(CancelBookingRequest request, ServerCallContext context)
  {
      try
      {
          // TODO kolla om user är admin eller om userId är samma som bokningens userId 
          if (string.IsNullOrWhiteSpace(request.Id) || request == null)
          {
              return new CancelBookingReply
              {
                  Succeeded = false,
                  Message = "Invalid cancellation request sent."
              };
          }


          var bookingToCancel = await _bookingRepository.GetAsync(x => x.Id == request.Id);
          if (bookingToCancel == null)
          {
              return new CancelBookingReply { Succeeded = false, Message = "Booking to cancel could not be found", StatusCode = 404 };
          }

          var result = await _bookingRepository.CancelBookingById(x => x.Id == request.Id, request.StatusId);
          return result.Succeeded
              ? new CancelBookingReply { StatusCode = 200, Succeeded = true, Message = $"Booking with id {request.Id} is cancelled" }
              : new CancelBookingReply { Message = "Booking could not be cancelled", StatusCode = 500, Succeeded = false };


      }
      catch (Exception ex)
      {
          Debug.Write(ex.Message);
          return new CancelBookingReply
          {
              Succeeded = false,
              Message = ex.Message,
          };
      }
  }
  */
}


