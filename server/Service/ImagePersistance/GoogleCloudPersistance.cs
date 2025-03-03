using Common.ErrorMessages;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.TransferModels.Responses.BalanceDto;

namespace Service.ImagePersistance;

public class GoogleCloudPersistance : IImagePersistanceService
{
    private readonly StorageClient _storageClient;
    private readonly ILogger<GoogleCloudPersistance> _logger;
    private IOptions<AppOptions>  _appOptions;

    //TODO after testing remove the image ulpoad max counter
    private static int MaxImagesToUpload = 10;
    private static int currentCounter = 0;

    public GoogleCloudPersistance(IOptions<AppOptions> options, ILogger<GoogleCloudPersistance> logger)
    {
        _storageClient = StorageClient.Create(GoogleCredential.GetApplicationDefault());
        _logger = logger;
        _appOptions =options;
    }

    public async Task<UploadedCloudImageResponse> Upload(IFormFile file)
    {
        if (currentCounter == MaxImagesToUpload)
        {
            throw new ApplicationException("No more images to upload");
        }

        // var bucketName = Environment.GetEnvironmentVariable("GooGleBucket", EnvironmentVariableTarget.User);
        var bucketName = _appOptions.Value.Bucket;
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using (var stream = file.OpenReadStream())
        {
            var googleResponse = await _storageClient.UploadObjectAsync(bucketName, fileName,
                file.ContentType, stream);
            var response = new UploadedCloudImageResponse
            {
                Name = googleResponse.Name,
                Bucket = googleResponse.Bucket,
                TimeCreated = googleResponse.TimeCreatedDateTimeOffset!.Value.UtcDateTime,
                Updated = googleResponse.TimeCreatedDateTimeOffset.Value.UtcDateTime,
                MediaLink = googleResponse.MediaLink
            };

            if (response.Name == null)
            {
                throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.FileHandling));
            }
            return response;
        }
    }

    public Task<UpdatedBalanceResponse> Read(Guid userId, DateTime date)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Delete(string bucketName, string imageName)
    {
        try
        {
            await _storageClient.DeleteObjectAsync(bucketName, imageName);
            return true;
        }
        catch (Google.GoogleApiException e)
        {
            _logger.LogError("Google api exception occured" + "when deleting " + imageName);
            return false;
        }
    }
}