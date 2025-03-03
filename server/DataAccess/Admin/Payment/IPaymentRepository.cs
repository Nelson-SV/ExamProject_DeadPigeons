

using DataAccess.Models;

public interface IPaymentRepository
{
    Dictionary<Payment, string> GetUserPendingPayments(int page, int pageSize, out int totalPendingPayments);
    Task UpdatePayments(Payment payment);
    Task<string> DeclinePendingPayments(Payment payment);
}