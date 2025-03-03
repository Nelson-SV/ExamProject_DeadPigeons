using Service.TransferModels.Requests;
using Service.TransferModels.Responses.BalanceDto;

public interface IUpdateBalance
{
    Task<ProcessedPaymentDto> RegisterPaymentWitTransactionImage(UpdateBalanceDto updtaBalanceDto, UploadedCloudImageResponse uplaodedCloudImageResponse);
    Task<bool> ValidateUser(string userId);
    Task<ProcessedPaymentDto> RegisterPaymentWithTransactionInput(string transactionId,UpdateBalanceDto updateBalanceRequest, UploadedCloudImageResponse uploadedCloudImageResponse);
    public Task<CurrentBalanceDto> RetrieveBalance(string userId);
}
