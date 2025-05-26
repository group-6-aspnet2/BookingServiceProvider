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

            booking.EventName = bookingEvent.EventName ?? "";
            booking.EventDate = DateOnly.Parse(bookingEvent.EventDate);
            booking.EventTime = TimeOnly.Parse(bookingEvent.EventTime);
            booking.EventCategoryName = bookingEvent.EventCategoryName ?? "";

            return booking;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null;
        }
    }
    public static BookingModel? MapAccountModelToBookingModel(BookingModel booking, AccountModel bookingAccount)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(booking);
            ArgumentNullException.ThrowIfNull(bookingAccount);

            booking.Email = bookingAccount.Email ?? "";
            booking.PhoneNumber = bookingAccount.PhoneNumber ?? "";

            return booking;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null;
        }
    }

    public static BookingModel? MapProfileModelToBookingModel(BookingModel booking, ProfileModel bookingProfile)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(booking);
            ArgumentNullException.ThrowIfNull(bookingProfile);

            booking.FirstName = bookingProfile.FirstName;
            booking.LastName = bookingProfile.LastName;
            return booking;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null;
        }
    }
}
