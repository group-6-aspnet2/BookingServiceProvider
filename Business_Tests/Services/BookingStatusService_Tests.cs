using Business.Services;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;
using Domain.Responses;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Linq.Expressions;

namespace Business_Tests.Services;

public class BookingStatusService_Tests
{
    private readonly IBookingStatusService _bookingStatusService;
    private readonly Mock<IBookingStatusRepository> _bookingStatusRepository = new();
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

    public BookingStatusService_Tests()
    {
        _bookingStatusService = new BookingStatusService(_bookingStatusRepository.Object, _memoryCache);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnStatuses_WhenSuccess()
    {
        var expectedStatuses = new List<StatusModel>
        {
            new() { Id = 1, StatusName = "Confirmed" },
            new() { Id = 2, StatusName = "Cancelled" }
        };

        _bookingStatusRepository
            .Setup(r => r.GetAllAsync(false, null, null, 0, Array.Empty<Expression<Func<StatusEntity, object>>>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<StatusModel>>
            {
                Succeeded = true,
                Result = expectedStatuses
            });

        var result = await _bookingStatusService.GetAllAsync();

        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(expectedStatuses.Count, result.Result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCachedStatuses_IfPresent()
    {
        var cacheKey = "StatusTypes";
        var expectedStatuses = new List<StatusModel>
    {
        new() { Id = 1, StatusName = "CachedStatus" }
    };

        _memoryCache.Set(cacheKey, expectedStatuses, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        var result = await _bookingStatusService.GetAllAsync();

        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("CachedStatus", result.Result.First().StatusName);

        _bookingStatusRepository.Verify(r => r.GetAllAsync(false, null, null, 0, Array.Empty<Expression<Func<StatusEntity, object>>>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnStatus_WhenFound()
    {
        var status = new StatusModel { Id = 1, StatusName = "Confirmed" };

        _bookingStatusRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<StatusEntity, bool>>>()))
            .ReturnsAsync(new RepositoryResult<StatusModel>
            {
                Succeeded = true,
                Result = status
            });

        var result = await _bookingStatusService.GetByIdAsync(1);

        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);

        Assert.Equal("Confirmed", result.Result.StatusName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBadRequest_WhenIdIsInvalid()
    {
        var result = await _bookingStatusService.GetByIdAsync(0);

        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Invalid ID provided", result.Error);

        _bookingStatusRepository.Verify(r => r.GetAsync(It.IsAny<Expression<Func<StatusEntity, bool>>>()), Times.Never);
    }
}
