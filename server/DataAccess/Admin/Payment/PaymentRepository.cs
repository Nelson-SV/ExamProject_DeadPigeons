
using Common.ErrorMessages;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<PaymentRepository> _logger;


    public PaymentRepository(AppDbContext context, ILogger<PaymentRepository> logger)
    {
        _context = context;
        _logger = logger;

    }

    public Dictionary<Payment, string> GetUserPendingPayments(int page, int pageSize, out int totalPendingPayments)
    {
        try
        {
            totalPendingPayments = _context.Payments
                .Count(p => p.Pending == true);

            var payments = _context.Payments
                .Where(p => p.Pending == true)
                .OrderByDescending(p => p.TimeCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var userIds = payments.Select(p => p.UserId).Distinct().ToList();

            /*Fetch UserNames based on UserIds*/
            var userNames = _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u.UserName);


            /* Create a dictionary with Payment and UserName*/
            var paymentsWithUserNames = payments.ToDictionary(
                payment => payment,
                payment => userNames.TryGetValue(payment.UserId, out var userName) ? userName : "Unknown"
            );
            Console.WriteLine(paymentsWithUserNames.Values);

            return paymentsWithUserNames;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessages.GetMessage(ErrorCode.RetrievingPaymentsFailed));
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.RetrievingPaymentsFailed), ex);
        }
       
    }

    public async Task UpdatePayments(Payment payment)

    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await UpdatePaymentStatus(payment.Guid);
            await UpdateUserBalance(payment.UserId, payment.Value);
            await InsertUpdatedDate(payment.Updated, payment.Guid);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, ErrorMessages.GetMessage(ErrorCode.UpdatingPaymentStatusFailed));
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.UpdatingPaymentStatusFailed), ex);
        }
    }

    private async Task InsertUpdatedDate(DateTime updated, String paymentId)
    {
        var payment = await _context.Payments
            .Where(p => p.Guid == paymentId)
            .FirstOrDefaultAsync();
        
        payment.Updated = updated;
        _context.Update(payment);
        await _context.SaveChangesAsync();

    }

    private async Task UpdatePaymentStatus(string paymentId)
    {
        var payment = await _context.Payments
            .Where(p => p.Guid == paymentId)
            .Where(p => p.Pending == true)
            .FirstOrDefaultAsync();
        if (payment == null)
        {
            throw new ApplicationException("Payment not found.");
        }
            
        payment.Pending = false;
        _context.Update(payment);
        await _context.SaveChangesAsync();

        
    }

    private async Task UpdateUserBalance(string userId, int amount)
    {
        var balance = await _context.Balances
            .Where(b => b.UserId == userId)
            .FirstOrDefaultAsync();

        if (balance == null)
        {
            _logger.LogError(ErrorMessages.GetMessage(ErrorCode.BalanceEmpty));
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.BalanceEmpty));
        }
        balance.Value += amount;
        _context.Update(balance);
        await _context.SaveChangesAsync();
        
    }
    
    public async Task<string> DeclinePendingPayments(Payment payment)
    {
        await using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                await DeletePayment(payment.Guid);
                var email = await GetUserEmail(payment.UserId);
                await transaction.CommitAsync();

                return email;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, ErrorMessages.GetMessage(ErrorCode.DecliningPayment));
                throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.DecliningPayment), ex);
            }
        }
    }

    public async Task<string> GetUserEmail(string userId)
    {
        var email = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Email) 
            .FirstOrDefaultAsync();
        if (email == null)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.EmailNotFound));
        }
        return email;
    }

    public async Task DeletePayment(string paymentId)
    {
        var payment = await _context.Payments
            .Where(p => p.Guid == paymentId)
            .FirstOrDefaultAsync();
        if (payment == null)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.PaymentNotFound));
        }
        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();

    }
}