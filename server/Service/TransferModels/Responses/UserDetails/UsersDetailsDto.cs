using DataAccess.Models;

namespace Service.TransferModels.Responses.UserDetails;

public class UsersDetailsDto
{
    public string UserId { get; set; }
    public string? Name { get; set; }    
    public string? Email { get; set; }   
    public string? PhoneNumber { get; set; }   
    public bool? IsActive { get; set; }   
    
    public static UsersDetailsDto FromEntity(UserProfile user)
    {
        return new UsersDetailsDto
        {
            UserId = user.UserId,
            Name = user.UserName,
            Email = user.User.Email,
            PhoneNumber = user.User.PhoneNumber,
            IsActive = user.isactive
        };
    }
    
}
