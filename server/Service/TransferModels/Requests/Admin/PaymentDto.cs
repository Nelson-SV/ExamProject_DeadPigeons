using DataAccess.Models;

namespace Service.TransferModels.Requests.Admin;

public class PaymentDto
{
    public string Guid { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string Bucket { get; set; } = null!;
    
    public DateTime TimeCreated { get; set; }
    
    public string MediaLink { get; set; } = null!;
    
    public string TransactionId { get; set; } = null!;
    
    public int Value { get; set; }
    
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    
    
    public static PaymentDto FromEntity(Payment payment, string userName)
    {
        return new PaymentDto
        {
            Guid = payment.Guid,
            Name = payment.Name,
            TimeCreated = payment.TimeCreated,
            MediaLink = payment.MediaLink,
            TransactionId = payment.TransactionId,
            Value = payment.Value,
            UserId = payment.UserId,
            UserName = userName,
            Bucket =payment.Bucket

        };

    }
    public static Payment ToEntity(PaymentDto paymentDto)
    {
        return new Payment
        {
            Guid = paymentDto.Guid,
            Name = paymentDto.Name,
            TimeCreated = paymentDto.TimeCreated,
            MediaLink = paymentDto.MediaLink,
            TransactionId = paymentDto.TransactionId
        };
    }

}