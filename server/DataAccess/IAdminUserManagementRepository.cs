using DataAccess.Models;

namespace DataAccess;

public interface IAdminUserManagementRepository
{
    List<UserProfile> GetAllUsersDetails(int page, int pageSize, out int totalUsers);
    Task<UserProfile> AddUserProfileAsync(UserProfile userProfile);
    Task<UserProfile> UpdateUserProfile(UserProfile userProfile);
    Task<bool> DeleteUserAsync(string userId);
}