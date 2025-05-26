using Domain.Models;

namespace Business.Interfaces;

public interface IEmailRestService
{
    Task SendBookingConfirmationAsync(BookingConfirmationRequest request);
}

