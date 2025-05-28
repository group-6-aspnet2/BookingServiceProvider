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


            if (!DateOnly.TryParse(bookingEvent.EventDate, out var eventDate))
                throw new FormatException("Invalid EventDate format");

            if (!TimeOnly.TryParse(bookingEvent.EventTime, out var eventTime))
                throw new FormatException("Invalid EventTime format");

            booking.EventName = bookingEvent.EventName ?? "";
            booking.EventDate = eventDate;     
            booking.EventTime = eventTime;  
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
