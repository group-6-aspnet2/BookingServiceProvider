using Azure.Messaging.ServiceBus;
using Business.Interfaces;
using Domain.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Presentation.AzureFunctions.Functions;

public class BookingQueueService
{
    private readonly ILogger<BookingQueueService> _logger;
    private readonly IBookingService _bookingService;
    public BookingQueueService(ILogger<BookingQueueService> logger, IBookingService bookingService)
    {
        _logger = logger;
        _bookingService = bookingService;
    }

    [Function(nameof(BookingQueueService))]
    public async Task Run([ServiceBusTrigger("update-booking-with-invoice-id", Connection = "ServiceBus")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message Body: {body}", message.Body);
        var body = message.Body.ToString();
        var form = JsonSerializer.Deserialize<UpdateBookingInvoiceIdForm>(body);

        if(form != null)
        {
            var result = await _bookingService.UpdateBookingInvoiceIdAsync(form);
        }

        await messageActions.CompleteMessageAsync(message);
    }
}
