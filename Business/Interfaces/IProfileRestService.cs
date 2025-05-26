using Domain.Models;

namespace Business.Interfaces;

public interface IProfileRestService
{
    Task<ProfileModel> GetProfileByIdAsync(string userId);
}
