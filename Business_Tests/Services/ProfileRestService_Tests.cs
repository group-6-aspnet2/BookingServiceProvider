using Business.Services;
using Domain.Models;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Business_Tests.Services;

public class ProfileRestService_Tests
{

    [Fact]
    public async Task GetProfileByIdAsync_ValidUserId_ReturnsProfile()
    {
        var userId = "user123";
        var expectedProfile = new ProfileModel { Id = userId, FirstName = "Hans", LastName = "Mattin-Lassei" };
        var profileJson = JsonSerializer.Serialize(expectedProfile);

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith($"api/profile/{userId}")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(profileJson)
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://fake-api.com/")
        };

        var service = new ProfileRestService(httpClient);

        var result = await service.GetProfileByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(expectedProfile.Id, result.Id);
        Assert.Equal(expectedProfile.FirstName, result.FirstName);
    }

    [Fact]
    public async Task GetProfileByIdAsync_NonSuccessStatusCode_ReturnsNull()
    {
        // Arrange
        var userId = "user123";

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://fake-api.com/")
        };

        var service = new ProfileRestService(httpClient);

        // Act
        var result = await service.GetProfileByIdAsync(userId);

        // Assert
        Assert.Null(result);
    }
}

