using DataAccess.Models;

namespace Service.TransferModels.Requests.Admin;

public class UpdatePaymentDto
{
    public string Guid { get; set; } = null!;
    
    public int Value { get; set; }
    
    public String UserId { get; set; } = null!;
    
    public DateTime Updated { get; set; } 
    
    public static Payment ToEntity(UpdatePaymentDto updatePaymentDto)
    {
        return new Payment
        {
            Guid = updatePaymentDto.Guid,
            Value = updatePaymentDto.Value,
            UserId = updatePaymentDto.UserId,
            Updated = DateTime.UtcNow
           
        };
    }
    
}