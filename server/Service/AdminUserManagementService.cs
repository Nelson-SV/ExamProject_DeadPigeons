using Common.EmailTemplates;
using Common.ErrorMessages;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Service.AdminService;
using Service.TransferModels.Requests;
using Service.TransferModels.Requests.Admin;
using Service.TransferModels.Responses;
using Service.TransferModels.Responses.UserDetails;

namespace Service;

public class AdminUserManagementService(
    IAdminUserManagementRepository adminUserManagementRepository,
    EmailService emailSender,
    UserManager<User> userManager
    ) : IAdminUserManagementService
{
    public UsersDetailsPageResultDto GetUsersDetails(string adminId, int page, int pageSize)
    {
        if (string.IsNullOrEmpty(adminId))
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.UserId));
        }

        var users = adminUserManagementRepository.GetAllUsersDetails(page, pageSize, out int totalUsers);
        
        if (users.Count == 0)
        {
            return new UsersDetailsPageResultDto
            {
                Items = new List<UsersDetailsDto>(),
                TotalItems = 0,
                Page = page,
                PageSize = 1
            };
        }

        var mappedUsers = users.Select(u =>
        {
            var detailedUser = UsersDetailsDto.FromEntity(u);
            return detailedUser;
        }).ToList();

        return new UsersDetailsPageResultDto()
        {
            Items = mappedUsers,
            TotalItems = totalUsers,
            Page = page,
            PageSize = pageSize,
        };

    }
    
    public async Task<UsersDetailsDto> AddUserProfile(CreateUserProfileDto userProfileProfiledto)
    {
        if (userProfileProfiledto == null)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.InvalidUserProfile));
        }

        var userProfile = CreateUserProfileDto.ToEntity(userProfileProfiledto);
        var addedUserProfile = await adminUserManagementRepository.AddUserProfileAsync(userProfile);
        
        return UsersDetailsDto.FromEntity(addedUserProfile);
    }
    
    public async Task<UsersDetailsDto> UpdateUserProfile(UpdateUserDto updateUserDto)
    {
        if (updateUserDto == null || string.IsNullOrWhiteSpace(updateUserDto.UserId))
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.InvalidUserProfile));
        }

        var updatedUserEntity = UpdateUserDto.ToEntity(updateUserDto);
        var updatedProfile = await adminUserManagementRepository.UpdateUserProfile(updatedUserEntity);

        return UsersDetailsDto.FromEntity(updatedProfile);
    }
    
    public async Task DeleteUserAsync(string userId)
    {
        if (userId == null)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.UserId));
        }
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.UserNotFound));
        }

        var result = await adminUserManagementRepository.DeleteUserAsync(userId);
        
        if(!result)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.UnexpectedError));
        }
    }

    
    public async Task SendResetEmail(string userName, string email, string confirmationLink)
    {
        try
        {
            // Issues with the path of the email templates (macOS vs Windows).
            // var template = reader.LoadTemplate("PasswordResetTemplate.html");
             var template = emailSender.LoadEmailTemplate("PasswordResetTemplate.html");

            
            var emailBody = template.Replace("{{CustomerName}}", userName)
                .Replace("{{ResetLink}}", confirmationLink);
        
            await emailSender.SendEmailAsync(email, "Password Reset Link", emailBody);
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException(ex.Message, ex);
        }
        
    }


    
}