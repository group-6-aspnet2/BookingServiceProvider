using Azure.Messaging.ServiceBus;
using Business.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Business.Services;


// Följande bakgrundsservice är från chatGpt för att lyssna och hantera meddelanden från en Azure Service Bus-kö. Vill uppdatera boknings invoiceId när en invoice har skapats
public class UpdateBookingQueueBackgroundService : BackgroundService
{

    private readonly ServiceBusProcessor _processor;
    private readonly ILogger<UpdateBookingQueueBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public UpdateBookingQueueBackgroundService(ServiceBusClient client, IConfiguration config, IServiceScopeFactory scopeFactory, ILogger<UpdateBookingQueueBackgroundService> logger)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

        _processor = client.CreateProcessor(config["AzureServiceBusSettings:UpdateBookingInvoiceQueueName"], new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += HandleMessageAsync;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task HandleMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            var payload = JsonSerializer.Deserialize<UpdateBookingInvoiceIdForm>(body);

            using var scope = _scopeFactory.CreateScope();
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

            var result = await bookingService.UpdateBookingInvoiceIdAsync(payload!);

            if (result.Succeeded)
            {
                await args.CompleteMessageAsync(args.Message);
            }
            else
            {
                _logger.LogWarning("Booking update failed: {error}", result.Error);
                await args.AbandonMessageAsync(args.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message");
            await args.DeadLetterMessageAsync(args.Message, "ProcessingError", ex.Message);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Message handler encountered an exception");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
