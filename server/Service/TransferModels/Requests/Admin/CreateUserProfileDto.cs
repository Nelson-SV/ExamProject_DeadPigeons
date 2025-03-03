using DataAccess.Models;

namespace Service.TransferModels.Requests.Admin;

public class CreateUserProfileDto
{
    public string UserId { get; set; }
    public string? Name { get; set; }    
    public bool? IsActive { get; set; }   
    
    public static CreateUserProfileDto FromEntity(UserProfile user)
    {
        return new CreateUserProfileDto
        {
            UserId = user.UserId,
            Name = user.UserName,
            IsActive = user.isactive
        };
    }
    
    public static UserProfile ToEntity(CreateUserProfileDto userProfile)
    {
        return new UserProfile
        {
            UserId = userProfile.UserId,
            UserName = userProfile.Name,
            isactive = userProfile.IsActive,
        };
    }
}