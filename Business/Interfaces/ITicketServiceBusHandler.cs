namespace Business.Interfaces;

public interface ITicketServiceBusHandler
{
    Task PublishAsync(string payload);
}
