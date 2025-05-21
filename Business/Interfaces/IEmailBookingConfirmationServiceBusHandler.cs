namespace Business.Interfaces;

public interface IEmailBookingConfirmationServiceBusHandler
{
    Task PublishAsync(string payload);
}
