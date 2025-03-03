using DataAccess.Models;
using DataAccess.QuerryModels.Balance.Response;


namespace DataAccess.BalanceRepository;


public interface IBalanceRepository
{
    // Task<ProcessedPaymentDto> UpdateBalance (Payment payment);
    Task<bool> ValidateUser(string userId);
    Task<ProcessedPaymentDto> RegisterPayment(Payment payment);
    
    Task<CurrentBalanceValueDto> RetrieveBalance(string userId);
}