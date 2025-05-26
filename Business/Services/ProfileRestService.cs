using Business.Interfaces;
using Domain.Models;
using System.Diagnostics;
using System.Text.Json;

namespace Business.Services;

public class ProfileRestService(HttpClient httpClient) : IProfileRestService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<ProfileModel> GetProfileByIdAsync(string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                ArgumentNullException.ThrowIfNullOrWhiteSpace(userId);

            var profileResponse = await _httpClient.GetAsync($"api/profile/{userId}");
            if (!profileResponse.IsSuccessStatusCode)
                return null!;

            var profileString = await profileResponse.Content.ReadAsStringAsync();
            var profile = JsonSerializer.Deserialize<ProfileModel>(profileString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return profile!;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null!;
        }
    }
}
