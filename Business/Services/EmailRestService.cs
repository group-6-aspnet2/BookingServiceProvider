using Business.Interfaces;
using Domain.Models;
using System.Net.Http.Json;

namespace Business.Services;

public class EmailRestService(HttpClient httpClient) : IEmailRestService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task SendBookingConfirmationAsync(BookingConfirmationRequest request)
    {

        var response = await _httpClient.PostAsJsonAsync("api/booking", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Kunde inte skicka mejl. Status: {response.StatusCode}, Fel: {error}");
        }
    }

}

