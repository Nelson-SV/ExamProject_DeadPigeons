using Service.TransferModels.Requests.Admin;
using Service.TransferModels.Responses;
using Service.TransferModels.Responses.UserDetails;

namespace Service;

public interface IAdminUserManagementService
{
    UsersDetailsPageResultDto GetUsersDetails(string adminId, int page, int pageSize);
    Task<UsersDetailsDto> AddUserProfile(CreateUserProfileDto userProfileProfile);
    Task<UsersDetailsDto> UpdateUserProfile(UpdateUserDto updateUserDto);
    Task SendResetEmail(string userName, string email, string confirmationLink);
    Task DeleteUserAsync(string userId);

}