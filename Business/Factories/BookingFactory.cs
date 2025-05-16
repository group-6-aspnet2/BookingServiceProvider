using Domain.Models;
using System.Diagnostics;

namespace Business.Factories;

public static class BookingFactory
{
    public static BookingModel? MapEventToBookingModel(BookingModel booking, Event bookingEvent)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(booking);
            ArgumentNullException.ThrowIfNull(bookingEvent);

            booking.EventName = bookingEvent.EventName;
            booking.EventDate = DateOnly.Parse(bookingEvent.EventDate);
            booking.EventTime = TimeOnly.Parse(bookingEvent.EventTime);
            booking.EventCategoryName = bookingEvent.EventCategoryName;

            return booking;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null;
        }
    }
    public static BookingModel? MapUserToBookingModel(BookingModel booking, User bookingUser)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(booking);
            ArgumentNullException.ThrowIfNull(bookingUser);

            booking.FirstName = bookingUser.FirstName;
            booking.LastName = bookingUser.LastName;
            booking.Email = bookingUser.Email;
            booking.PhoneNumber = bookingUser.PhoneNumber;

            return booking;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null;
        }
    }
}
