using DataAccess.Models;

namespace Service.TransferModels.Requests.Admin;

public class UpdateUserDto
{
    public string UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    
    public static UserProfile ToEntity(UpdateUserDto userProfile)
    {
        return new UserProfile
        {
            UserId = userProfile.UserId,
            UserName = userProfile.Name,
            isactive = userProfile.IsActive,
        };
    }
}