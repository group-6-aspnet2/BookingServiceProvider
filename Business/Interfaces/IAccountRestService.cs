using Domain.Models;

namespace Business.Interfaces;

public interface IAccountRestService
{
    Task<AccountModel> GetAccountByIdAsync(string userId);
}
