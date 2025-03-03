using Microsoft.AspNetCore.Http;
using Service.TransferModels.Responses.BalanceDto;
namespace Service.ImagePersistance;

public interface IImagePersistanceService
{
    Task<UploadedCloudImageResponse> Upload(IFormFile file);
    Task<UpdatedBalanceResponse> Read(Guid userId, DateTime date);
    Task<bool> Delete(string bucketName, string imageName);
}