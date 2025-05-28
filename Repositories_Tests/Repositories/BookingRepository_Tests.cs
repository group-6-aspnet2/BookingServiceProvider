using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Domain.Models;
using Repositories_Tests.SeedData;

namespace Repositories_Tests.Repositories;

public class BookingRepository_Tests
{

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBookings_IfSucceeded()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);

        await context.SaveChangesAsync();

        var bookingRepository = new BookingRepository(context);

        var result = await bookingRepository.GetAllAsync();

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(TestData.BookingEntities.Length, result.Result.Count());
        await context.DisposeAsync();

    }

    [Fact]
    public async Task GetAsync_ShouldReturnBookingModel_WhenSuccess()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);

        await context.SaveChangesAsync();

        var bookingRepository = new BookingRepository(context);

        var result = await bookingRepository.GetAsync(x => x.Id == "832697b2-57b7-4c5c-a033-f444f5415588");

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal("832697b2-57b7-4c5c-a033-f444f5415588", result.Result.Id);
        await context.DisposeAsync();

    }


    [Fact]
    public async Task GetAsync_ShouldReturnNotFound_WhenBookingDoesNotExists()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);

        await context.SaveChangesAsync();

        var bookingRepository = new BookingRepository(context);

        var result = await bookingRepository.GetAsync(x => x.Id == "bokiningsid-som-icke-finns123");

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        await context.DisposeAsync();

    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenBookingExists()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);

        await context.SaveChangesAsync();

        var bookingRepository = new BookingRepository(context);

        var result = await bookingRepository.GetAsync(x => x.Id == "832697b2-57b7-4c5c-a033-f444f5415588");

        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        await context.DisposeAsync();

    }


    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenBookingDoesNotExists()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);

        await context.SaveChangesAsync();

        var bookingRepository = new BookingRepository(context);


        var result = await bookingRepository.GetAsync(x => x.Id == "bokiningsid-som-icke-finns123");

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        await context.DisposeAsync();

    }

    [Fact]
    public async Task AddAsync_ShouldAddBooking_ReturnModelIfSuccess()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);

        await context.SaveChangesAsync();
        var bookingRepository = new BookingRepository(context);

        var bookingEntity = new BookingEntity {
        Id= "ett-id-123",
        StatusId = 1,
        EventId = "eventId-123",
        UserId = "userid-123",
        TicketCategoryName = "VIP",
        TicketPrice = 100,
        TicketQuantity = 1,
        CreateDate = DateTime.Now,
        };

        var result =await bookingRepository.AddAsync(bookingEntity);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(201, result.StatusCode);

        await context.DisposeAsync();
    }


    [Fact]
    public async Task AddAsync_ShouldReturnSuccededFalse_WhenNullbleEntity()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);

        await context.SaveChangesAsync();
        var bookingRepository = new BookingRepository(context);

        BookingEntity bookingEntity = null!;

        var result = await bookingRepository.AddAsync(bookingEntity);

        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        await context.DisposeAsync();

    }


    [Fact]
    public async Task UpdateBookingFromModelAsync_ShouldUpdateBookingWithModel_ReturnUpdatedModel()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);

        await context.SaveChangesAsync();
        var bookingRepository = new BookingRepository(context);

        var model = new BookingModel
        {
            Id = "832697b2-57b7-4c5c-a033-f444f5415588",
            StatusId = 2,
            EventId = "a2beec0a-7ef7-4617-9901-50cfc5fab680",
            InvoiceId = "c9755db1-c394-4e84-b425-b4d1801ef079",
            UserId = "52df0a8f-66b1-4fcd-85ca-63bb95634526",
            TicketCategoryName = "Platinum",
            TicketPrice = 700,
            TicketQuantity = 3,
            CreateDate = DateTime.Parse("2025-04-20 14:00:00")
        };

        var result = await bookingRepository.UpdateBookingFromModelAsync(model);

        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        await context.DisposeAsync();

    }

    [Fact]
    public async Task UpdateBookingFromModelAsync_ShouldNotFindBooking_RetyrbFalseWhenNotFound()
    {
               var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();
        var bookingRepository = new BookingRepository(context);
        var model = new BookingModel
        {
            Id = "id-som-icke-existerar",
            StatusId = 2,
            EventId = "a2beec0a-7ef7-4617-9901-50cfc5fab680",
            InvoiceId = "c9755db1-c394-4e84-b425-b4d1801ef079",
            UserId = "52df0a8f-66b1-4fcd-85ca-63bb95634526",
            TicketCategoryName = "Platinum",
            TicketPrice = 700,
            TicketQuantity = 3,
            CreateDate = DateTime.Parse("2025-04-20 14:00:00")
        };
        var result = await bookingRepository.UpdateBookingFromModelAsync(model);
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        await context.DisposeAsync();
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotFindBooking_ReturnSuccededFalse()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);

        await context.SaveChangesAsync();
        var bookingRepository = new BookingRepository(context);

        var entity = new BookingEntity
        {
            Id = "id-som-icke-existerar",
            StatusId = 2,
            EventId = "a2beec0a-7ef7-4617-9901-50cfc5fab680",
            InvoiceId = "c9755db1-c394-4e84-b425-b4d1801ef079",
            UserId = "52df0a8f-66b1-4fcd-85ca-63bb95634526",
            TicketCategoryName = "Platinum",
            TicketPrice = 700,
            TicketQuantity = 3,
            CreateDate = DateTime.Parse("2025-04-20 14:00:00")
        };

        var result = await bookingRepository.UpdateAsync(entity);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        await context.DisposeAsync();

    }


    [Fact]
    public async Task DeleteAsync_ShouldDeleteBooking_ReturnTrueIfSuccess()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();
        var bookingRepository = new BookingRepository(context);
        var result = await bookingRepository.DeleteAsync(x => x.Id == "832697b2-57b7-4c5c-a033-f444f5415588");
        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
        await context.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotFindBooking_ReturnFalseIfNotFound()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();
        var bookingRepository = new BookingRepository(context);
        var result = await bookingRepository.DeleteAsync(x => x.Id == "bokiningsid-som-icke-finns123");
        
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        await context.DisposeAsync();
    }

    [Fact]
    public async Task GetAsyncWithStatus_ShouldReturnBookingsWithStatuses_WhenIncludeStatus()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();

        var bookingRepository = new BookingRepository(context);
        var result = await bookingRepository.GetAsyncWithStatus(x => x.Id == "832697b2-57b7-4c5c-a033-f444f5415588", b => b.Status);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal("832697b2-57b7-4c5c-a033-f444f5415588", result.Result.Id);
        Assert.Equal("Pending", result.Result.StatusName);
    }

    [Fact]
    public async Task GetAllAsyncWithStatus_ShouldReturnBookingsWithStatuses_WhenIncludeStatus()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Bookings.AddRange(TestData.BookingEntities);
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();
        
        var bookingRepository = new BookingRepository(context);
        var result = await bookingRepository.GetAllAsyncWithStatus();
       
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(TestData.BookingEntities.Length, result.Result.Count());
        Assert.Equal("Pending", result.Result.First().StatusName);

    }
}
