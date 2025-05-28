using Business;
using Business.Interfaces;
using Business.Services;
using Business_Tests.TestHelpers;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;
using Moq;
using System.Linq.Expressions;

namespace Business_Tests.Services;

public class BookingService_Tests
{
    private readonly Mock<IBookingRepository> _bookingRepository = new();
    private readonly Mock<IBookingStatusRepository> _statusRepository = new();
    private readonly Mock<EventContract.EventContractClient> _eventClient = new();
    private readonly Mock<IInvoiceServiceBusHandler> _invoiceServiceBus = new();
    private readonly Mock<ITicketServiceBusHandler> _ticketServiceBusHandler = new();
    private readonly Mock<IEmailRestService> _emailService = new();
    private readonly Mock<IAccountRestService> _accountRestService = new();
    private readonly Mock<IProfileRestService> _profileRestService = new();
    private readonly IBookingService _bookingService;


    public BookingService_Tests()
    {
        _bookingService = new BookingService(
            _bookingRepository.Object,
            _statusRepository.Object,
            _eventClient.Object,
            _invoiceServiceBus.Object,
            _ticketServiceBusHandler.Object,
            _emailService.Object,
            _accountRestService.Object,
            _profileRestService.Object
        );
    }

    [Fact]
    public async Task GetOneAsync_ShouldReturnBookingModel_WhenAllDependenciesSucceed()
    {
        var bookingId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();

        var booking = new BookingModel
        {
            Id = bookingId,
            UserId = userId,
            EventId = eventId,
            StatusName = "Confirmed"
        };

        var eventReply = new GetEventByIdReply
        {
            Succeeded = true,
            Event = new Event
            {
                EventId = eventId,
                EventName = "Test Event",
                EventDate = "2025-05-27",
                EventTime = "14:30",
                EventCategoryName = "Music"
            }
        };

        var account = new AccountModel
        {
            Id = userId,
            UserName = "anna@example.com",
            Email = "anna@example.com",
            PhoneNumber = "0701234567"
        };

        var profile = new ProfileModel
        {
            Id = userId,
            FirstName = "Anna",
            LastName = "Andersson"
        };

        _bookingRepository.Setup(repo => repo.GetAsyncWithStatus(
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<BookingModel>
            {
                Succeeded = true,
                Result = booking,
                StatusCode = 200
            });

        _eventClient.Setup(client => client.GetEventByIdAsync(
            It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(eventReply));

        _accountRestService.Setup(x => x.GetAccountByIdAsync(userId))
            .ReturnsAsync(account);

        _profileRestService.Setup(x => x.GetProfileByIdAsync(userId))
            .ReturnsAsync(profile);

        var result = await _bookingService.GetOneAsync(bookingId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Equal(bookingId, result.Result.Id);
        Assert.Equal(eventId, result.Result.EventId);
        Assert.Equal("Test Event", result.Result.EventName);
        Assert.Equal("Anna", result.Result.FirstName);
        Assert.Equal("0701234567", result.Result.PhoneNumber);
        Assert.Equal("Confirmed", result.Result.StatusName);
    }

    [Fact]
    public async Task GetOneAsync_ShouldReturnNotFound_WhenBookingNotFound()
    {
        var bookingId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();

        _bookingRepository.Setup(repo => repo.GetAsyncWithStatus(
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<BookingModel>
            {
                Succeeded = true,
                Result = null,
                StatusCode = 200
            });

        _eventClient.Setup(client => client.GetEventByIdAsync(
    It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
    .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
    {
        Succeeded = true,
        Event = new Event
        {
            EventId = eventId,
            EventName = "Mock Event",
            EventDate = "2025-01-01",
            EventTime = "12:00",
            EventCategoryName = "Music"
        }
    }));

        var result = await _bookingService.GetOneAsync(bookingId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Booking not found", result.Error);
    }
    [Fact]
    public async Task GetOneAsync_ShouldReturn404_WhenAccountNotFound()
    {
        var bookingId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();

        var booking = new BookingModel { Id = bookingId, UserId = userId, EventId = eventId };

        _bookingRepository.Setup(repo => repo.GetAsyncWithStatus(
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<BookingModel>
            {
                Succeeded = true,
                Result = booking,
                StatusCode = 200
            });

        _eventClient.Setup(client => client.GetEventByIdAsync(
    It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
    .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
    {
        Succeeded = true,
        Event = new Event
        {
            EventId = eventId,
            EventName = "Mock Event",
            EventDate = "2025-01-01",
            EventTime = "12:00",
            EventCategoryName = "Music"
        }
    }));

        _accountRestService.Setup(x => x.GetAccountByIdAsync(userId))!
            .ReturnsAsync((AccountModel?)null);

        var result = await _bookingService.GetOneAsync(bookingId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Could not find account with provided Id", result.Error);
    }
    [Fact]
    public async Task GetOneAsync_ShouldReturnNotFound_WhenProfileNotFound()
    {
        var bookingId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();

        var booking = new BookingModel { Id = bookingId, UserId = userId, EventId = eventId };

        _bookingRepository.Setup(repo => repo.GetAsyncWithStatus(
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<BookingModel>
            {
                Succeeded = true,
                Result = booking,
                StatusCode = 200
            });

        _eventClient.Setup(client => client.GetEventByIdAsync(
    It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
    .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
    {
        Succeeded = true,
        Event = new Event
        {
            EventId = eventId,
            EventName = "Mock Event",
            EventDate = "2025-01-01",
            EventTime = "12:00",
            EventCategoryName = "Music"
        }
    }));

        _accountRestService.Setup(x => x.GetAccountByIdAsync(userId))
            .ReturnsAsync(new AccountModel { Id = userId });

        _profileRestService.Setup(x => x.GetProfileByIdAsync(userId))!
            .ReturnsAsync((ProfileModel?)null);

        var result = await _bookingService.GetOneAsync(bookingId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Could not find profile with provided Id", result.Error);
    }
    [Fact]
    public async Task GetOneAsync_ShouldReturnNotFound_WhenEventFetchFails()
    {
        var bookingId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();

        var booking = new BookingModel { Id = bookingId, UserId = userId, EventId = eventId };

        _bookingRepository.Setup(repo => repo.GetAsyncWithStatus(
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<BookingModel>
            {
                Succeeded = true,
                Result = booking,
                StatusCode = 200
            });

        _accountRestService.Setup(x => x.GetAccountByIdAsync(userId))
            .ReturnsAsync(new AccountModel { Id = userId });

        _profileRestService.Setup(x => x.GetProfileByIdAsync(userId))
            .ReturnsAsync(new ProfileModel { Id = userId });

        _eventClient.Setup(client => client.GetEventByIdAsync(
            It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = false
            }));

        var result = await _bookingService.GetOneAsync(bookingId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Could not fetch event", result.Error);
    }

    [Fact]
    public async Task GetAllBookingsAsync_ShouldReturnBookings_WhenAllDependenciesSucceed()
    {
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();
        var booking = new BookingModel { Id = Guid.NewGuid().ToString(), UserId = userId, EventId = eventId, StatusName = "Confirmed" };

        _bookingRepository.Setup(repo => repo.GetAllAsyncWithStatus(true, It.IsAny<Expression<Func<BookingEntity, object>>>(), null, 0, It.IsAny<Expression<Func<BookingEntity, object>>[]>())).ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
        {
            Succeeded = true,
            Result = new List<BookingModel> { booking },
            StatusCode = 200
        });

        _eventClient.Setup(client => client.GetEventByIdAsync(
            It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = true,
                Event = new Event
                {
                    EventId = eventId,
                    EventName = "Sample Event",
                    EventDate = "2025-06-01",
                    EventTime = "18:00",
                    EventCategoryName = "Music"
                }
            }));

        _accountRestService.Setup(x => x.GetAccountByIdAsync(userId))
            .ReturnsAsync(new AccountModel
            {
                Id = userId,
                UserName = "user@example.com",
                PhoneNumber = "0701234567"
            });

        _profileRestService.Setup(x => x.GetProfileByIdAsync(userId))
            .ReturnsAsync(new ProfileModel
            {
                Id = userId,
                FirstName = "Anna",
                LastName = "Andersson"
            });

        var result = await _bookingService.GetAllBookingsAsync();

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.NotEmpty(result.Result);
        var single = result.Result.First();
        Assert.Equal(userId, single.UserId);
        Assert.Equal(eventId, single.EventId);
        Assert.Equal("Anna", single.FirstName);
    }


    [Fact]
    public async Task GetAllBookingsAsync_ShouldReturn404_WhenAccountNotFound()
    {
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();

        var booking = new BookingModel
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            EventId = eventId,
            StatusName = "Confirmed"
        };

        _bookingRepository.Setup(repo => repo.GetAllAsyncWithStatus(true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(), null, 0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = true,
                Result = new List<BookingModel> { booking },
                StatusCode = 200
            });

        _eventClient.Setup(client => client.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = true,
                Event = new Event
                {
                    EventId = eventId,
                    EventName = "Sample Event",
                    EventDate = "2025-06-01",
                    EventTime = "18:00",
                    EventCategoryName = "Music"
                }
            }));

        _accountRestService.Setup(x => x.GetAccountByIdAsync(userId))!
            .ReturnsAsync((AccountModel?)null);

        var result = await _bookingService.GetAllBookingsAsync();

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Could not find account with provided Id", result.Error);
    }

    [Fact]
    public async Task GetAllBookingsAsync_ShouldReturn404_WhenProfileNotFound()
    {
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();

        var booking = new BookingModel
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            EventId = eventId,
            StatusName = "Confirmed"
        };

        _bookingRepository.Setup(repo => repo.GetAllAsyncWithStatus(true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(), null, 0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = true,
                Result = new List<BookingModel> { booking },
                StatusCode = 200
            });

        _eventClient.Setup(client => client.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = true,
                Event = new Event
                {
                    EventId = eventId,
                    EventName = "Sample Event",
                    EventDate = "2025-06-01",
                    EventTime = "18:00",
                    EventCategoryName = "Music"
                }
            }));

        _accountRestService.Setup(x => x.GetAccountByIdAsync(userId))
            .ReturnsAsync(new AccountModel
            {
                Id = userId,
                UserName = "user@example.com",
                PhoneNumber = "0701234567"
            });

        _profileRestService.Setup(x => x.GetProfileByIdAsync(userId))!
            .ReturnsAsync((ProfileModel?)null);

        var result = await _bookingService.GetAllBookingsAsync();

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Could not find profile with provided Id", result.Error);
    }
    [Fact]
    public async Task GetAllBookingsAsync_ShouldReturn404_WhenEventFetchFails()
    {
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();

        var booking = new BookingModel
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            EventId = eventId,
            StatusName = "Confirmed"
        };

        _bookingRepository.Setup(repo => repo.GetAllAsyncWithStatus(true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(), null, 0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = true,
                Result = new List<BookingModel> { booking },
                StatusCode = 200
            });

        _eventClient.Setup(client => client.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = false
            }));

        var result = await _bookingService.GetAllBookingsAsync();

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Could not fetch event", result.Error);
    }

    [Fact]
    public async Task GetBookingsByEventIdAsync_ShouldReturnFilteredBookings_WhenAllDependenciesSucceed()
    {
        var eventId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var booking = new BookingModel { Id = Guid.NewGuid().ToString(), UserId = userId, EventId = eventId, StatusName = "Confirmed" };

        _bookingRepository.Setup(repo => repo.GetAllAsyncWithStatus(
        true,
        It.IsAny<Expression<Func<BookingEntity, object>>>(),
        It.IsAny<Expression<Func<BookingEntity, bool>>>(),
        0,
        It.IsAny<Expression<Func<BookingEntity, object>>[]>()
        ))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = true,
                Result = new List<BookingModel> { booking },
                StatusCode = 200
            });

        _eventClient.Setup(client => client.GetEventByIdAsync(
            It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = true,
                Event = new Event
                {
                    EventId = eventId,
                    EventName = "Filtered Event",
                    EventDate = "2025-07-01",
                    EventTime = "19:00",
                    EventCategoryName = "Theater"
                }
            }));

        _accountRestService.Setup(x => x.GetAccountByIdAsync(userId))
            .ReturnsAsync(new AccountModel
            {
                Id = userId,
                UserName = "filtereduser@example.com",
                PhoneNumber = "0707654321"
            });

        _profileRestService.Setup(x => x.GetProfileByIdAsync(userId))
            .ReturnsAsync(new ProfileModel
            {
                Id = userId,
                FirstName = "Erik",
                LastName = "Eriksson"
            });

        var result = await _bookingService.GetBookingsByEventIdAsync(eventId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        var bookingResult = result.Result.First();
        Assert.Equal(eventId, bookingResult.EventId);
        Assert.Equal("Erik", bookingResult.FirstName);
        Assert.Equal("Filtered Event", bookingResult.EventName);
    }

    [Fact]
    public async Task GetBookingsByEventIdAsync_ShouldReturnNotFound_WhenEventNotFound()
    {
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();
        var booking = new BookingModel { Id = Guid.NewGuid().ToString(), UserId = userId, EventId = eventId };

        _bookingRepository.Setup(repo => repo.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = true,
                Result = new List<BookingModel> { booking },
                StatusCode = 200
            });

        _eventClient.Setup(client => client.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = false
            }));

        var result = await _bookingService.GetBookingsByEventIdAsync(eventId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetBookingsByEventIdAsync_ShouldReturnNotFound_WhenAccountNotFound()
    {
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();
        var booking = new BookingModel { Id = Guid.NewGuid().ToString(), UserId = userId, EventId = eventId };

        _bookingRepository.Setup(repo => repo.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = true,
                Result = new List<BookingModel> { booking },
                StatusCode = 200
            });

        _eventClient.Setup(client => client.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = true,
                Event = new Event
                {
                    EventId = eventId,
                    EventDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    EventTime = DateTime.Now.ToString("HH:mm:ss"),
                }
            }));

        _accountRestService.Setup(x => x.GetAccountByIdAsync(userId))!
            .ReturnsAsync((AccountModel?)null);

        var result = await _bookingService.GetBookingsByEventIdAsync(eventId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Could not find account with provided Id", result.Error);
    }

    [Fact]
    public async Task GetBookingsByEventIdAsync_ShouldReturn404_WhenProfileNotFound()
    {
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();
        var booking = new BookingModel { Id = Guid.NewGuid().ToString(), UserId = userId, EventId = eventId };

        _bookingRepository.Setup(repo => repo.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = true,
                Result = new List<BookingModel> { booking },
                StatusCode = 200
            });

        _eventClient.Setup(client => client.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = true,
                Event = new Event
                {
                    EventId = eventId,
                    EventDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    EventTime = DateTime.Now.ToString("HH:mm:ss"),
                }
            }));

        _accountRestService.Setup(x => x.GetAccountByIdAsync(userId))
            .ReturnsAsync(new AccountModel { Id = userId });

        _profileRestService.Setup(x => x.GetProfileByIdAsync(userId))!
            .ReturnsAsync((ProfileModel?)null);

        var result = await _bookingService.GetBookingsByEventIdAsync(eventId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Could not find profile with provided Id", result.Error);
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_ShouldReturnSuccess_WhenDataExists()
    {
        var userId = Guid.NewGuid().ToString();
        var booking = new BookingModel { Id = Guid.NewGuid().ToString(), UserId = userId, EventId = "event1", StatusName = "Confirmed" };

        _bookingRepository.Setup(r => r.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = true,
                Result = new List<BookingModel> { booking },
                StatusCode = 200
            });

        _eventClient.Setup(c => c.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = true,
                Event = new Event { EventId = "event1", EventName = "Sample Event", EventDate = "2025-06-01", EventTime = "18:00", EventCategoryName = "Music" }
            }));

        _accountRestService.Setup(s => s.GetAccountByIdAsync(userId))
            .ReturnsAsync(new AccountModel { Id = userId, UserName = "user@example.com", PhoneNumber = "0701234567" });

        _profileRestService.Setup(s => s.GetProfileByIdAsync(userId))
            .ReturnsAsync(new ProfileModel { Id = userId, FirstName = "Anna", LastName = "Andersson" });

        var result = await _bookingService.GetBookingsByUserIdAsync(userId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Single(result.Result);
        Assert.Equal(userId, result.Result.First().UserId);
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_ShouldReturnNotFound_WhenAccountNotFound()
    {
        var userId = Guid.NewGuid().ToString();
        var booking = new BookingModel { Id = Guid.NewGuid().ToString(), UserId = userId, EventId = "event1" };

        _bookingRepository.Setup(r => r.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = true,
                Result = new List<BookingModel> { booking },
                StatusCode = 200
            });

        _eventClient.Setup(c => c.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = true,
                Event = new Event { EventId = "event1" }
            }));

        _accountRestService.Setup(s => s.GetAccountByIdAsync(userId))!
            .ReturnsAsync((AccountModel?)null);

        var result = await _bookingService.GetBookingsByUserIdAsync(userId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Could not find account with provided Id", result.Error);
    }


    [Fact]
    public async Task GetBookingsByUserIdAsync_ShouldReturnNotFound_WhenBookingResultIsNull()
    {
        var userId = Guid.NewGuid().ToString();

        _bookingRepository.Setup(r => r.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync((RepositoryResult<IEnumerable<BookingModel>>)null!);

        var result = await _bookingService.GetBookingsByUserIdAsync(userId);

        Assert.False(result.Succeeded);
        Assert.Equal(500, result.StatusCode);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_ShouldReturnFailed_WhenBookingResultIsFailed()
    {
        var userId = Guid.NewGuid().ToString();

        _bookingRepository.Setup(r => r.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = false,
                StatusCode = 400,
                Error = "Bad request"
            });

        var result = await _bookingService.GetBookingsByUserIdAsync(userId);

        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Bad request", result.Error);
    }

    [Fact]
    public async Task GetBookingsByStatusIdAsync_ShouldReturnSuccess_WhenDataExists()
    {
        int statusId = 1;
        var userId = Guid.NewGuid().ToString();
        var booking = new BookingModel { Id = Guid.NewGuid().ToString(), UserId = userId, EventId = "event1", StatusId = statusId, StatusName = "Confirmed" };

        _bookingRepository.Setup(r => r.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = true,
                Result = new List<BookingModel> { booking },
                StatusCode = 200
            });

        _eventClient.Setup(c => c.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = true,
                Event = new Event { EventId = "event1", EventName = "Sample Event", EventDate = "2025-06-01", EventTime = "18:00", EventCategoryName = "Music" }
            }));

        _accountRestService.Setup(s => s.GetAccountByIdAsync(userId))
            .ReturnsAsync(new AccountModel { Id = userId, UserName = "user@example.com", PhoneNumber = "0701234567" });

        _profileRestService.Setup(s => s.GetProfileByIdAsync(userId))
            .ReturnsAsync(new ProfileModel { Id = userId, FirstName = "Anna", LastName = "Andersson" });

        var result = await _bookingService.GetBookingsByStatusIdAsync(statusId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Result);
        Assert.Single(result.Result);
        Assert.Equal(statusId, result.Result.First().StatusId);
    }

    [Fact]
    public async Task GetBookingsByStatusIdAsync_ShouldReturnNotFound_WhenProfileNotFound()
    {
        int statusId = 1;
        var userId = Guid.NewGuid().ToString();
        var booking = new BookingModel { Id = Guid.NewGuid().ToString(), UserId = userId, EventId = "event1", StatusId = statusId };

        _bookingRepository.Setup(r => r.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = true,
                Result = new List<BookingModel> { booking },
                StatusCode = 200
            });

        _eventClient.Setup(c => c.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(new GetEventByIdReply
            {
                Succeeded = true,
                Event = new Event { EventId = "event1" }
            }));

        _accountRestService.Setup(s => s.GetAccountByIdAsync(userId))
            .ReturnsAsync(new AccountModel { Id = userId });

        _profileRestService.Setup(s => s.GetProfileByIdAsync(userId))!
            .ReturnsAsync((ProfileModel?)null);

        var result = await _bookingService.GetBookingsByStatusIdAsync(statusId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Could not find profile with provided Id", result.Error);
    }


    [Fact]
    public async Task GetBookingsByStatusIdAsync_ShouldReturnNotFound_WhenBookingResultIsNull()
    {
        int statusId = 1;

        _bookingRepository.Setup(r => r.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync((RepositoryResult<IEnumerable<BookingModel>>)null!);

        var result = await _bookingService.GetBookingsByStatusIdAsync(statusId);

        Assert.False(result.Succeeded);
        Assert.Equal(500, result.StatusCode);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task GetBookingsByStatusIdAsync_ShouldReturnFailed_WhenBookingResultIsFailed()
    {
        int statusId = 1;

        _bookingRepository.Setup(r => r.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = false,
                StatusCode = 400,
                Error = "Bad request"
            });

        var result = await _bookingService.GetBookingsByStatusIdAsync(statusId);

        Assert.False(result.Succeeded);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Bad request", result.Error);
    }

    [Fact]
    public async Task CreateNewBookingAsync_ShouldReturnSuccess_WhenValidForm()
    {
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();
        var bookingId = Guid.NewGuid().ToString();
        var booking = new BookingModel { Id = bookingId, UserId = userId, EventId = eventId, StatusName = "Confirmed" };

        var form = new CreateBookingForm
        {
            UserId = userId,
            EventId = eventId,
            TicketQuantity = 2,
            TicketPrice = 100,
            TicketCategoryName = "VIP"
        };

        var eventReply = new GetEventByIdReply
        {
            Event = new Event
            {
                EventId = form.EventId,
                EventAmountOfGuests = 10,
                EventDate = DateTime.Now.ToString("yyyy-MM-dd"),
                EventTime = DateTime.Now.ToString("HH:mm:ss"),
                EventName = "TestEvent",
                EventCategoryName = "TestCategory"
            }
        };

        var account = new AccountModel
        {
            Id = form.UserId,
            Email = "user@example.com",
            UserName = "user@example.com",
            PhoneNumber = ""
        };

        var profile = new ProfileModel
        {
            Id = form.UserId,
            FirstName = "Anna",
            LastName = "Andersson"
        };

        var repositoryResult = new RepositoryResult<BookingModel>
        {
            Succeeded = true,
            Result = new BookingModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = form.UserId,
                EventId = form.EventId,
                TicketQuantity = form.TicketQuantity,
                StatusId = 1,
                CreateDate = DateTime.UtcNow,
            },
            StatusCode = 201
        };

        _bookingRepository.Setup(repo => repo.GetAllAsyncWithStatus(
       true,
       It.IsAny<Expression<Func<BookingEntity, object>>>(),
       It.IsAny<Expression<Func<BookingEntity, bool>>>(),
       0,
       It.IsAny<Expression<Func<BookingEntity, object>>[]>()
       ))
           .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
           {
               Succeeded = true,
               Result = new List<BookingModel> { booking },
               StatusCode = 200
           });

        _bookingRepository.Setup(x => x.AddAsync(It.IsAny<BookingEntity>()))
            .ReturnsAsync(repositoryResult);

        _eventClient.Setup(x => x.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(eventReply));

        _accountRestService.Setup(x => x.GetAccountByIdAsync(form.UserId))
            .ReturnsAsync(account);

        _profileRestService.Setup(x => x.GetProfileByIdAsync(form.UserId))
            .ReturnsAsync(profile);

        _invoiceServiceBus.Setup(x => x.PublishAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _ticketServiceBusHandler.Setup(x => x.PublishAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _emailService.Setup(x => x.SendBookingConfirmationAsync(It.IsAny<BookingConfirmationRequest>())).Returns(Task.CompletedTask);

        var result = await _bookingService.CreateNewBookingAsync(form);

        Assert.True(result.Succeeded);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(form.UserId, result.Result.UserId);
        Assert.Equal(form.EventId, result.Result.EventId);
    }

    [Fact]
    public async Task CreateNewBookingAsync_ShouldReturnNotFound_WhenAccountNotFound()
    {
        var form = new CreateBookingForm
        {
            UserId = Guid.NewGuid().ToString(),
            EventId = Guid.NewGuid().ToString(),
            TicketQuantity = 1
        };

        var eventReply = new GetEventByIdReply
        {
            Event = new Event { EventId = form.EventId, EventAmountOfGuests = 10 }
        };

        _eventClient.Setup(x => x.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(eventReply));


        _accountRestService.Setup(x => x.GetAccountByIdAsync(form.UserId))!
            .ReturnsAsync((AccountModel?)null);

        var result = await _bookingService.CreateNewBookingAsync(form);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Account with provided ID could not be found.", result.Error);
    }

    [Fact]
    public async Task CreateNewBookingAsync_ShouldReturnInvalidForm_WhenFormIsNull()
    {
        var result = await _bookingService.CreateNewBookingAsync(null!);

        Assert.False(result.Succeeded);
        Assert.Equal("Invalid new booking form", result.Error);
    }


    [Fact]
    public async Task UpdateBookingAsync_ShouldReturnSuccess_WhenValidForm()
    {
        var bookingId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();

        var form = new UpdateBookingForm
        {
            Id = bookingId,
            UserId = userId,
            EventId = eventId,
            TicketQuantity = 2
        };

        var existingBooking = new BookingModel
        {
            Id = bookingId,
            UserId = userId,
            EventId = eventId,
            TicketQuantity = 1,
            StatusId = 1
        };

        var eventReply = new GetEventByIdReply
        {
            Event = new Event
            {
                EventId = eventId,
                EventAmountOfGuests = 10,
                EventDate = DateTime.Now.ToString("yyyy-MM-dd"),
                EventTime = DateTime.Now.ToString("HH:mm:ss"),
                EventName = "TestEvent",
                EventCategoryName = "Standard"
            }
        };

        var account = new AccountModel
        {
            Id = userId,
            Email = "user@example.com",
            PhoneNumber = "1234567890"
        };

        var profile = new ProfileModel
        {
            Id = userId,
            FirstName = "Anna",
            LastName = "Andersson"
        };

        var updatedBooking = new BookingModel
        {
            Id = bookingId,
            UserId = userId,
            EventId = eventId,
            TicketQuantity = 2
        };



        _bookingRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<BookingEntity, bool>>>()))
            .ReturnsAsync(new RepositoryResult<bool> { Succeeded = true, Result = true });

        _bookingRepository.Setup(repo => repo.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()
        ))
            .ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
            {
                Succeeded = true,
                Result = new List<BookingModel> { existingBooking },
                StatusCode = 200
            });

        _eventClient.Setup(x => x.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(eventReply));

        _accountRestService.Setup(x => x.GetAccountByIdAsync(userId))
            .ReturnsAsync(account);

        _profileRestService.Setup(x => x.GetProfileByIdAsync(userId))
            .ReturnsAsync(profile);

        var updateResult = new BookingResult<BookingModel>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = updatedBooking
        };

        _bookingRepository.Setup(x => x.UpdateBookingFromModelAsync(It.IsAny<BookingModel>()))
            .ReturnsAsync(updateResult);

        var result = await _bookingService.UpdateBookingAsync(form);

        Assert.True(result.Succeeded);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(form.Id, result.Result.Id);
        Assert.Equal(form.TicketQuantity, result.Result.TicketQuantity);
    }

    [Fact]
    public async Task UpdateBookingAsync_ShouldReturnError_WhenBookingNotFoundOrTooManyTickets()
    {
        var form = new UpdateBookingForm
        {
            Id = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            EventId = Guid.NewGuid().ToString(),
            TicketQuantity = 5
        };

        _bookingRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<BookingEntity, bool>>>()))
            .ReturnsAsync(new RepositoryResult<bool> { Succeeded = false, Result = false });

        var eventReply = new GetEventByIdReply
        {
            Event = new Event
            {
                EventId = form.EventId,
                EventAmountOfGuests = 10,
                EventDate = DateTime.Now.ToString("yyyy-MM-dd"),
                EventTime = DateTime.Now.ToString("HH:mm:ss"),
                EventName = "Event Test",
                EventCategoryName = "Music"
            }
        };

        _eventClient.Setup(x => x.GetEventByIdAsync(It.IsAny<GetEventByIdRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(GrpcTestHelpers.CreateAsyncUnaryCall(eventReply));

        var existingBookings = new List<BookingModel>
    {
        new BookingModel { Id = Guid.NewGuid().ToString(), EventId = form.EventId, TicketQuantity = 10 }
    };

        _bookingRepository.Setup(repo => repo.GetAllAsyncWithStatus(
            true,
            It.IsAny<Expression<Func<BookingEntity, object>>>(),
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            0,
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()
        )).ReturnsAsync(new RepositoryResult<IEnumerable<BookingModel>>
        {
            Succeeded = true,
            Result = existingBookings
        });

        var result = await _bookingService.UpdateBookingAsync(form);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"Could not find a booking with id {form.Id}.", result.Error);
    }

    [Fact]
    public async Task CancelBookingAsync_ShouldReturnSuccess_WhenBookingIsCancelled()
    {
        var bookingId = Guid.NewGuid().ToString();

        var booking = new BookingModel
        {
            Id = bookingId,
            StatusId = 1
        };

        var cancelledStatus = new StatusModel
        {
            Id = 2,
            StatusName = "Cancelled"
        };

        _bookingRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<BookingEntity, bool>>>()))
            .ReturnsAsync(new RepositoryResult<BookingModel> { Succeeded = true, Result = booking });

        _statusRepository.Setup(repo =>
       repo.GetAllAsync(
           It.IsAny<bool>(),
           It.IsAny<Expression<Func<StatusEntity, object>>>(),
           It.IsAny<Expression<Func<StatusEntity, bool>>>(),
           It.IsAny<int>(),
           It.IsAny<Expression<Func<StatusEntity, object>>[]>()
       ))
       .ReturnsAsync(new RepositoryResult<IEnumerable<StatusModel>>
       {
           Succeeded = true,
           Result = new List<StatusModel>
           {
            new StatusModel { Id = 2, StatusName = "Cancelled" }
           }
       });

        _bookingRepository.Setup(repo => repo.UpdateBookingFromModelAsync(It.IsAny<BookingModel>()))
            .ReturnsAsync(new BookingResult<BookingModel> { Succeeded = true });

        var result = await _bookingService.CancelBookingAsync(bookingId);

        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
    }


    [Fact]
    public async Task CancelBookingAsync_ShouldReturnNotFound_WhenBookingDoesNotExist()
    {
        var bookingId = Guid.NewGuid().ToString();

        _bookingRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<BookingEntity, bool>>>()))
            .ReturnsAsync(new RepositoryResult<BookingModel> { Succeeded = false, Result = null });

        var result = await _bookingService.CancelBookingAsync(bookingId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Could not find a booking", result.Error);
    }

    [Fact]
    public async Task DeleteBookingWithTickets_ShouldReturnSuccess_WhenBookingIsDeleted()
    {
        var bookingId = Guid.NewGuid().ToString();

        var booking = new BookingModel { Id = bookingId };

        _bookingRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<BookingEntity, bool>>>()))
            .ReturnsAsync(new RepositoryResult<BookingModel> { Succeeded = true, Result = booking });

        _bookingRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Expression<Func<BookingEntity, bool>>>()))
            .ReturnsAsync(new RepositoryResult { Succeeded = true });

        var result = await _bookingService.DeleteBookingWithTickets(bookingId);

        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
    }

    [Fact]
    public async Task DeleteBookingWithTickets_ShouldReturnNotFound_WhenBookingDoesNotExist()
    {
        var bookingId = Guid.NewGuid().ToString();

        _bookingRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<BookingEntity, bool>>>()))
            .ReturnsAsync(new RepositoryResult<BookingModel> { Succeeded = false, Result = null });

        var result = await _bookingService.DeleteBookingWithTickets(bookingId);

        Assert.False(result.Succeeded);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Could not find a booking", result.Error);
    }

    [Fact]
    public async Task UpdateBookingInvoiceIdAsync_ShouldReturnSuccess_WhenValidForm()
    {
        var bookingId = Guid.NewGuid().ToString();
        var invoiceId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var eventId = Guid.NewGuid().ToString();

        var form = new UpdateBookingInvoiceIdForm
        {
            BookingId = bookingId,
            InvoiceId = invoiceId
        };

        var existingBookingModel = new BookingModel
        {
            Id = bookingId,
            InvoiceId = "",
            UserId = userId,
            EventId = eventId
        };

        var updatedBookingModel = existingBookingModel.MapTo<BookingModel>();
        updatedBookingModel.InvoiceId = invoiceId;

        var repositoryUpdateResult = new BookingResult<BookingModel>
        {
            Succeeded = true,
            StatusCode = 204,
            Result = existingBookingModel
        };
        var repositoryGetResult = new RepositoryResult<BookingModel>
        {
            Succeeded = true,
            Result = updatedBookingModel
        };

        _bookingRepository.Setup(x => x.GetAsync(
            It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            It.IsAny<Expression<Func<BookingEntity, object>>[]>()
        )).ReturnsAsync(repositoryGetResult);

        _bookingRepository.Setup(x => x.UpdateBookingFromModelAsync(It.IsAny<BookingModel>()))
            .ReturnsAsync(repositoryUpdateResult);

        var result = await _bookingService.UpdateBookingInvoiceIdAsync(form);

        Assert.True(result.Succeeded);
        Assert.Equal(204, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(invoiceId, result.Result.InvoiceId);
    }

    [Fact]
    public async Task UpdateBookingInvoiceIdAsync_ShouldReturnError_WhenFormIsInvalid()
    {
        UpdateBookingInvoiceIdForm nullForm = null!;

        var emptyBookingIdForm = new UpdateBookingInvoiceIdForm
        {
            BookingId = "",
            InvoiceId = "some-invoice"
        };

        var emptyInvoiceIdForm = new UpdateBookingInvoiceIdForm
        {
            BookingId = "some-booking",
            InvoiceId = ""
        };

        var resultNull = await _bookingService.UpdateBookingInvoiceIdAsync(nullForm);
        var resultEmptyBookingId = await _bookingService.UpdateBookingInvoiceIdAsync(emptyBookingIdForm);
        var resultEmptyInvoiceId = await _bookingService.UpdateBookingInvoiceIdAsync(emptyInvoiceIdForm);

        Assert.False(resultNull.Succeeded);
        Assert.Equal("Invalid ids provided", resultNull.Error);

        Assert.False(resultEmptyBookingId.Succeeded);
        Assert.Equal("Invalid ids provided", resultEmptyBookingId.Error);

        Assert.False(resultEmptyInvoiceId.Succeeded);
        Assert.Equal("Invalid ids provided", resultEmptyInvoiceId.Error);
    }

    [Fact]
    public async Task UpdateBookingInvoiceIdAsync_ShouldReturnError_WhenBookingNotFound()
    {
        var form = new UpdateBookingInvoiceIdForm
        {
            BookingId = "not-found-id",
            InvoiceId = "some-invoice"
        };

        var repositoryGetResult = new RepositoryResult<BookingModel>
        {
            Succeeded = false,
            Result = null,
            Error = "Booking not found"
        };

        _bookingRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<BookingEntity, bool>>>(),
            It.IsAny<Expression<Func<BookingEntity, object>>[]>())).ReturnsAsync(repositoryGetResult);

        var result = await _bookingService.UpdateBookingInvoiceIdAsync(form);

        Assert.False(result.Succeeded);
        Assert.Equal("Could not find a booking with the provided id", result.Error);
    }


}