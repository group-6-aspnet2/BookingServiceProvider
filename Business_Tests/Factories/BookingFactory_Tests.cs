using Business;
using Business.Factories;
using Domain.Models;

namespace Business_Tests.Factories;

public class BookingFactory_Tests
{

    [Fact]
    public void MapEventToBookingModel_ShouldMapEventDetailsToBookingModel()
    {
        var booking = new BookingModel();
        var bookingEvent = new Event
        {
            EventName = "Test Event",
            EventDate = "2025-05-27",
            EventTime = "14:30",
            EventCategoryName = "Music"
        };

        var result = BookingFactory.MapEventToBookingModel(booking, bookingEvent);

        Assert.NotNull(result);
        Assert.Equal("Test Event", result.EventName);
        Assert.Equal(new DateOnly(2025, 5, 27), result.EventDate);
        Assert.Equal(new TimeOnly(14, 30), result.EventTime);
        Assert.Equal("Music", result.EventCategoryName);
    }


    [Fact]
    public void MapAccountModelToBookingModel_ShouldMapAccountDetailsToBookingModel()
    {
        var booking = new BookingModel();
        var account = new AccountModel
        {
            Email = "test@example.com",
            PhoneNumber = "123456789"
        };

        var result = BookingFactory.MapAccountModelToBookingModel(booking, account);

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("123456789", result.PhoneNumber);
    }

    [Fact]
    public void MapProfileModelToBookingModel_ShouldMapProfileDetailsToBookingModel()
    {
        var booking = new BookingModel();
        var profile = new ProfileModel
        {
            FirstName = "Anna",
            LastName = "Andersson"
        };

        var result = BookingFactory.MapProfileModelToBookingModel(booking, profile);

        Assert.NotNull(result);
        Assert.Equal("Anna", result.FirstName);
        Assert.Equal("Andersson", result.LastName);
    }

    [Fact]
    public void MapEventToBookingModel_ShouldReturnNull_WhenBookingIsNull()
    {
        var bookingEvent = new Event { EventName = "Test", EventDate = "2025-01-01", EventTime = "12:00", EventCategoryName = "Sport" };

        var result = BookingFactory.MapEventToBookingModel(null!, bookingEvent);

        Assert.Null(result);
    }
}
