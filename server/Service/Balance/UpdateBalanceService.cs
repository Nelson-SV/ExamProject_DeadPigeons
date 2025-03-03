using Common;
using DataAccess.BalanceRepository;
using DataAccess.Models;
using Service.TransferModels.Requests;
using Service.TransferModels.Responses.BalanceDto;
using ProcessedPaymentDto = Service.TransferModels.Responses.BalanceDto.ProcessedPaymentDto;

namespace Service.Balance;

public class UpdateBalanceService : IUpdateBalance
{
    private IBalanceRepository _balanceRepository;

    public UpdateBalanceService(IBalanceRepository balanceRepository)
    {
        _balanceRepository = balanceRepository;
    }

    public async Task<ProcessedPaymentDto> RegisterPaymentWitTransactionImage(UpdateBalanceDto updtateBalanceRequest,
        UploadedCloudImageResponse uplaodedCloudImageResponse)
    {
        var payment = new Payment
        {
            Guid = Guid.NewGuid().ToString(),
            Name = uplaodedCloudImageResponse.Name,
            Bucket = uplaodedCloudImageResponse.Bucket,
            TimeCreated = uplaodedCloudImageResponse.TimeCreated,
            Updated = uplaodedCloudImageResponse.Updated,
            MediaLink = uplaodedCloudImageResponse.MediaLink,
            UserId = updtateBalanceRequest.UserId,
            TransactionId = "",
            Pending = true,
            Value = updtateBalanceRequest.BalanceValue
        };
        var processedPayment = await _balanceRepository.RegisterPayment(payment);
        return new ProcessedPaymentDto
        {
            Registered = processedPayment.Registered,
            Message = processedPayment.Message
        };
    }


    public Task<bool> ValidateUser(string userId)
    {
        return _balanceRepository.ValidateUser(userId);
    }

    public async Task<ProcessedPaymentDto> RegisterPaymentWithTransactionInput(string transactionId,
        UpdateBalanceDto updateBalanceRequest
        , UploadedCloudImageResponse uploadedCloudImageResponse)
    {
        var payment = new Payment
        {
            Guid = Guid.NewGuid().ToString(),
            Name = uploadedCloudImageResponse.Name,
            Bucket = uploadedCloudImageResponse.Bucket,
            TimeCreated = uploadedCloudImageResponse.TimeCreated.ToUtc(),
            Updated = uploadedCloudImageResponse.Updated.ToUtc(),
            MediaLink = uploadedCloudImageResponse.MediaLink,
            UserId = updateBalanceRequest.UserId,
            TransactionId = transactionId,
            Pending = true,
            Value = updateBalanceRequest.BalanceValue
        };
        var processedPayment = await _balanceRepository.RegisterPayment(payment);
        return new ProcessedPaymentDto
        {
            Registered = processedPayment.Registered,
            Message = processedPayment.Message
        };
    }
         public async Task<CurrentBalanceDto> RetrieveBalance(string userId)
     {
         var currentBalance = await _balanceRepository.RetrieveBalance(userId);

         return new CurrentBalanceDto
         {
             BalanceValue = currentBalance.BalanceValue
         };
     }
}



