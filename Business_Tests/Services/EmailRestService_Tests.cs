
using Business.Services;
using Domain.Models;
using Moq;
using Moq.Protected;
using System.Net;

namespace Business_Tests.Services;

public class EmailRestService_Tests
{
    [Fact]
    public async Task SendBookingConfirmationAsync_SuccessfulResponse_ShouldNotThrow()
    {
        var request = new BookingConfirmationRequest
        {
            UserEmail = "test@example.com",
            BookingId = "abc123"
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m =>
                    m.Method == HttpMethod.Post &&
                    m.RequestUri!.ToString().EndsWith("api/booking")
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://fake-api.com/")
        };

        var service = new EmailRestService(httpClient);

        Exception? exception = await Record.ExceptionAsync(() => service.SendBookingConfirmationAsync(request));

        Assert.Null(exception);
    }

}
