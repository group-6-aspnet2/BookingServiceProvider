using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Business.Services;

public interface IInvoiceServiceBusHandler
{
    Task PublishAsync(string payload);
}

public class InvoiceServiceBusHandler : IInvoiceServiceBusHandler
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    public InvoiceServiceBusHandler(IConfiguration configuration)
    {
        _client = new ServiceBusClient(configuration["AzureServiceBusSettings:ConnectionString"]);
        _sender = _client.CreateSender(configuration["AzureServiceBusSettings:CreateInvoiceQueueName"]);
    }

    public async Task PublishAsync(string payload)
    {
        var message = new ServiceBusMessage(payload);
        Console.WriteLine($"Sending message: {payload}");
        await _sender.SendMessageAsync(message);
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}


