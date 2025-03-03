
using Common.ErrorMessages;
using Service.ImagePersistance;
using Service.TransferModels.Requests.Admin;

namespace Service.AdminService.Payment;

public class PaymentService (IPaymentRepository paymentRepository, EmailService _emailService, GoogleCloudPersistance googleCloud): IPaymentService
{
    /* This method retrieves a paginated list of pending payments.
     Key responsibilities:
     1. Fetch pending payments along with user IDs from the repository.
     2. Map the payments to DTOs.
     3. Return the paginated result as a PaymentPageResultDto object.*/

    public PaymentPageResultDto<PaymentDto> GetPendingPayments(int page, int pageSize)
    {
        var paymentsWithUserIds = paymentRepository.GetUserPendingPayments( page, pageSize, out int totalPendingPayments);
        var mappedPayments = paymentsWithUserIds.Select(paymentWithUserName =>
        {
            var payment = paymentWithUserName.Key;
            var userName = paymentWithUserName.Value;
            return PaymentDto.FromEntity(payment, userName); 
        }).ToList();

        return new PaymentPageResultDto<PaymentDto>
        {
            Items = mappedPayments,
            TotalItems = totalPendingPayments,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task UpdatePendingPayments( UpdatePaymentDto updatePaymentDto)
    {
        var updatedPayment = UpdatePaymentDto.ToEntity(updatePaymentDto);
        await paymentRepository.UpdatePayments(updatedPayment);
    }
    
     /*This method handles the process of declining a pending payment.
     Key responsibilities:
     1. Convert the incoming DTO to an entity representing the declined payment.
     2. Use the repository to mark the payment as declined.
     3. Delete the associated payment files from the cloud storage.
     4. Send an email notification to the user indicating the payment was declined.
     5. */
    public async Task DeclinePendingPayments(DeclinePaymentDto declinePaymentDto)
    {
        var declinedPayment = DeclinePaymentDto.ToEntity(declinePaymentDto);
       
        var userEmail = await paymentRepository.DeclinePendingPayments(declinedPayment);
        
        await googleCloud.Delete(declinedPayment.Bucket, declinedPayment.Name);
            
        var template = _emailService.LoadEmailTemplate("PaymentDeclined.html");
            
        var emailBody = template.Replace("{{CustomerName}}", declinePaymentDto.UserName);
        try
        {
            await _emailService.SendEmailAsync(userEmail, "Payment Declined", emailBody);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.EmailNotSent), ex);
        }
        
            
    }

}