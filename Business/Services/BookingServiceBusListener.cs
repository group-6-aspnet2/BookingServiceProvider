using Azure.Messaging.ServiceBus;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text.Json;

namespace Business.Services;

public interface IBookingServiceBusListener
{
    Task StartProcessingAsync();
}

public class InvoiceServiceBusReply
{
    public string BookingId { get; set; } = null!;
    public string InvoiceId { get; set; } = null!;
}
public class BookingServiceBusListener : IBookingServiceBusListener
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusProcessor _processor;
    private readonly IBookingRepository _bookingRepository;

    public BookingServiceBusListener(IConfiguration configuration, ServiceBusClient client, IBookingRepository bookingRepository)
    {
        _client = client;
        _bookingRepository = bookingRepository;
        _processor = _client.CreateProcessor(configuration["ServiceBus:UpdateBookingInvoiceQueueName"], new ServiceBusProcessorOptions());

        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;
    }

    public async Task StartProcessingAsync()
    {
        await _processor.StartProcessingAsync();
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"📨 Mottaget meddelande: {body}");

        try
        {
            var data = JsonSerializer.Deserialize<InvoiceServiceBusReply>(body);



            var updateResult = await UpdateBookingWithInvoiceId(data!.BookingId, data.InvoiceId);

            if (!updateResult.Succeeded)
            {
                Console.WriteLine($"❌ Misslyckades med att uppdatera bokning: {updateResult.Error}");
                await args.DeadLetterMessageAsync(args.Message);
                return;
            }
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid hantering av meddelande: {ex.Message}");
            await args.DeadLetterMessageAsync(args.Message);
        }
    }

    private async Task<BookingResult<BookingModel>> UpdateBookingWithInvoiceId(string bookingId, string invoiceId)
    {
        try
        {
            if (string.IsNullOrEmpty(bookingId) || string.IsNullOrEmpty(invoiceId))
                return new BookingResult<BookingModel> { Succeeded = false, Error = "Invalid booking id or invoice id" };


            var existingBooking = await _bookingRepository.GetAsync(x => x.Id == bookingId);
            var model = existingBooking.MapTo<BookingModel>();
            model.InvoiceId = invoiceId;

            var result = await _bookingRepository.UpdateBookingFromModelAsync(model);

            return new BookingResult<BookingModel> { Succeeded = true, StatusCode = result.StatusCode, Result = result.Result };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new BookingResult<BookingModel> { Succeeded = false, Error = ex.Message, StatusCode = 500 };
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Service Bus Error: {args.Exception}");
        return Task.CompletedTask;
    }
}
