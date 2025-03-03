using Service.TransferModels.Requests.Admin;

namespace Service.AdminService.Payment;

public interface IPaymentService
{
    PaymentPageResultDto<PaymentDto> GetPendingPayments(int page, int pageSize);
    Task UpdatePendingPayments( UpdatePaymentDto updatePaymentDto);

    Task DeclinePendingPayments(DeclinePaymentDto declinePaymentDto);
}