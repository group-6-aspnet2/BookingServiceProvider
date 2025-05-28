using Business.Services;
using Domain.Models;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Business_Tests.Services;

public class AccountRestService_Tests
{
    [Fact]
    public async Task GetAccountByIdAsync_ReturnsAccount_WhenApiReturnsSuccess()
    {
        var userId = Guid.NewGuid().ToString();
        var expectedAccount = new AccountModel { Id = userId, UserName = "hans@domain.com", Email = "hans@domain.com" };
        var json = JsonSerializer.Serialize(expectedAccount);

        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.Is<HttpRequestMessage>(req =>
                   req.Method == HttpMethod.Get &&
                   req.RequestUri!.ToString().EndsWith($"api/accounts/{userId}")
               ),
               ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(json)
           });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://fake-api.com/")
        };

        var service = new AccountRestService(httpClient);
        var result = await service.GetAccountByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(expectedAccount.Id, result.Id);
    }

}
