using Common.ErrorMessages;
using DataAccess.Models;
using DataAccess.QuerryModels.Balance.Request;
using DataAccess.QuerryModels.Balance.Response;
using Microsoft.EntityFrameworkCore;
using ApplicationException = Common.AplicationExceptions.ApplicationException;

namespace DataAccess.BalanceRepository;

public class BalanceRepository : IBalanceRepository
{
    private AppDbContext _appDbContext;

    public BalanceRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    // public async Task<CurrentBalanceValueDto> UpdateBalance(UpdateBalanceQto updateBalanceQto)
    // {
    //     var updateBalance = _appDbContext.Balances.FirstOrDefault(b => b.UserId.Equals(updateBalanceQto.UserId));
    //     updateBalance!.Value += updateBalanceQto.BalanceValue;
    //     _appDbContext.Update(updateBalance);
    //     await _appDbContext.SaveChangesAsync();
    //     var response = new CurrentBalanceValueDto
    //     {
    //         BalanceValue = updateBalance.Value
    //     };
    //     return response;
    // }


    public async Task<ProcessedPaymentDto> UpdateBalance(Payment payment)
    {
        CurrentBalanceValueDto response;
        await using (var transaction = _appDbContext.Database.BeginTransaction())
        {
            try
            {
                _appDbContext.Payments.Add(payment);
                await _appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ProcessedPaymentDto
                {
                    Registered = true,
                    Message = ErrorMessages.GetMessage(ErrorCode.SuccessRegisterPayment)
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return new ProcessedPaymentDto
                {
                    Registered = false,
                    Message = ErrorMessages.GetMessage(ErrorCode.ErrorRegisterPayment)
                };
            }
        }
    }


    public async Task<bool> ValidateUser(string userId)
    {
        var retrieveUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
        Console.WriteLine(retrieveUser);
        return retrieveUser != null;
    }

    public async Task<ProcessedPaymentDto> RegisterPayment(Payment payment)
    {
        _appDbContext.Payments.Add(payment);
        try
        {
            await _appDbContext.SaveChangesAsync();
            return new ProcessedPaymentDto
            {
                Registered = true,
                Message = ErrorMessages.GetMessage(ErrorCode.SuccessRegisterPayment)
            };
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return new ProcessedPaymentDto
            {
                Registered = false,
                Message = ErrorMessages.GetMessage(ErrorCode.ErrorRegisterPayment)
            };
        }
        catch (DbUpdateException ex)
        {
            return new ProcessedPaymentDto
            {
                Registered = false,
                Message = ErrorMessages.GetMessage(ErrorCode.ErrorRegisterPayment)
            };
        }
        catch (Exception ex)
        {
            return new ProcessedPaymentDto
            {
                Registered = false,
                Message = ErrorMessages.GetMessage(ErrorCode.ErrorRegisterPayment)
            };
        }
    }

    public async Task<CurrentBalanceValueDto> RetrieveBalance(string userId)
    {
        var balance = await _appDbContext.Balances.FirstOrDefaultAsync((b) => b.UserId == userId);
        return new CurrentBalanceValueDto
        {
            BalanceValue = balance.Value
        };
    }
}