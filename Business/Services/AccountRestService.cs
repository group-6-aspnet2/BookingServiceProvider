using Business.Interfaces;
using Domain.Models;
using System.Diagnostics;
using System.Text.Json;

namespace Business.Services;

public class AccountRestService(HttpClient httpClient) : IAccountRestService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<AccountModel> GetAccountByIdAsync(string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                ArgumentNullException.ThrowIfNullOrWhiteSpace(userId);

            var accountResponse = await _httpClient.GetAsync($"api/accounts/{userId}");
            if (!accountResponse.IsSuccessStatusCode)
                return null!;
            var accountString = await accountResponse.Content.ReadAsStringAsync();
            var account = JsonSerializer.Deserialize<AccountModel>(accountString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true 
            });

            return account!;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null!;
        }
    }
}
