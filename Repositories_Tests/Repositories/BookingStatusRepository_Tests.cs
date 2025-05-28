using Data.Entities;
using Data.Repositories;
using Repositories_Tests.SeedData;

namespace Repositories_Tests.Repositories;

public class BookingStatusRepository_Tests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBookingStatuses_IfSucceeded()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();

        var statusRepository = new BookingStatusRepository(context);

        var result = await statusRepository.GetAllAsync();

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(TestData.StatusEntities.Length, result.Result.Count());
        await context.DisposeAsync();
    }
    [Fact]
    public async Task GetAsync_ShouldReturnStatusModel_WhenSucceeded()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Statuses.AddRange(TestData.StatusEntities);

        await context.SaveChangesAsync();

        var statusRepository = new BookingStatusRepository(context);

        var result = await statusRepository.GetAsync(x => x.Id == 1);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(1, result.Result.Id);
        await context.DisposeAsync();

    }


    [Fact]
    public async Task GetAsync_ShouldReturnNotFound_WhenBookingStatusDoesNotExists()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Statuses.AddRange(TestData.StatusEntities);

        await context.SaveChangesAsync();

        var statusRepository = new BookingStatusRepository(context);

        var result = await statusRepository.GetAsync(x => x.Id == 112233);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        await context.DisposeAsync();

    }


    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenBookingStatusExists()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();

        var statusRepository = new BookingStatusRepository(context);

        var result = await statusRepository.GetAsync(x => x.Id == 1);

        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        await context.DisposeAsync();

    }


    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenBookingStatusDoesNotExists()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();

        var statusRepository = new BookingStatusRepository(context);


        var result = await statusRepository.GetAsync(x => x.Id == 443322);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        await context.DisposeAsync();

    }


    [Fact]
    public async Task AddAsync_ShouldAddBookingStatus_ReturnModelIfSuccess()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();
        var statusRepository = new BookingStatusRepository(context);

        var statusEntity = new StatusEntity
        {
            Id = 4,
            StatusName = "Completed"
        };
        var result = await statusRepository.AddAsync(statusEntity);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(201, result.StatusCode);

        await context.DisposeAsync();
    }


    [Fact]
    public async Task AddAsync_ShouldReturnSuccededFalse_WhenNullbleEntity()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();
        var statusRepository = new BookingStatusRepository(context);

        StatusEntity statusEntity = null!;

        var result = await statusRepository.AddAsync(statusEntity);
        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        await context.DisposeAsync();
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotFindBookingStatus_ReturnSuccededFalse()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();

        var statusRepository = new BookingStatusRepository(context);

        var statusEntity = new StatusEntity
        {
            Id = 12345,
            StatusName = "Updated Status"
        };

        var result = await statusRepository.UpdateAsync(statusEntity);
        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        await context.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteStatus_ReturnTrueIfSuccess()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();
        var statusRepository = new BookingStatusRepository(context);
        var result = await statusRepository.DeleteAsync(x => x.Id == 3);
        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
        await context.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotFindBooking_ReturnFalseIfNotFound()
    {
        var context = new DataContextSeeder().GetDataContext();
        context.Statuses.AddRange(TestData.StatusEntities);
        await context.SaveChangesAsync();
        var statusRepository = new BookingStatusRepository(context);
        var result = await statusRepository.DeleteAsync(x => x.Id == 1234);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        await context.DisposeAsync();
    }


}
