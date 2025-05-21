namespace Business.Interfaces;

public interface IInvoiceServiceBusHandler
{
    Task PublishAsync(string payload);
}


