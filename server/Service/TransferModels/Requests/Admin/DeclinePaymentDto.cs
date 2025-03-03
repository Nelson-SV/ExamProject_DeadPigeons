using DataAccess.Models;

namespace Service.TransferModels.Requests.Admin;

public class DeclinePaymentDto
{
    public string Guid { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string UserId { get; set; } = null!;
    
    public string Bucket { get; set; } = null!;
    
   public string UserName { get; set; } = null!;
    
    
    public static Payment ToEntity(DeclinePaymentDto declinePaymentDto)
    {
        return new Payment
        {
            Guid = declinePaymentDto.Guid, 
            Name = declinePaymentDto.Name,
            UserId = declinePaymentDto.UserId,
            Bucket = declinePaymentDto.Bucket,
           
        };
    }
}