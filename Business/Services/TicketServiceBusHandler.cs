using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Business.Services;

public interface ITicketServiceBusHandler
{
    Task PublishAsync(string payload);
}

public class TicketServiceBusHandler : ITicketServiceBusHandler
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    public TicketServiceBusHandler(IConfiguration configuration)
    {
        _client = new ServiceBusClient(configuration["ServiceBus:ConnectionString"]);
        _sender = _client.CreateSender(configuration["ServiceBus:CreateTicketQueueName"]);
    }

    public async Task PublishAsync(string payload)
    {
        var message = new ServiceBusMessage(payload);
        await _sender.SendMessageAsync(message);
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}
