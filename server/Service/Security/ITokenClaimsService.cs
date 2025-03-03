using DataAccess.Models;

namespace Service.Security;

public interface ITokenClaimsService
{
    Task<string> GetTokenAsync(string userName);
    // Task<string> GenerateToken(User user);
}