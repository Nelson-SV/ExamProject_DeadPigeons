using API.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.ImagePersistance;
using Service.TransferModels.Requests;
using Service.TransferModels.Responses.BalanceDto;


namespace Api.Controllers;

[ApiController]
[Route("/user/balance")]
public class PaymentController : ControllerBase
{
    /// <summary>
    /// Register a payment request into the database, and if case saves an image into the google cloud
    /// </summary>
    /// <param name="_imageService"></param>
    /// <param name="_updateBalance"></param>
    /// <param name="file"></param>
    /// <param name="topUpValue"></param>
    /// <param name="authUserId"></param>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("")]
    [Authorize(Roles = Role.Player)]
    public async Task<ActionResult<UpdatedBalanceResponse>> UploadImage(
        [FromServices] IImagePersistanceService _imageService,
        [FromServices] IUpdateBalance _updateBalance,
        [FromForm] IFormFile file,
        [FromForm] int topUpValue,
        [FromForm] string authUserId,
        [FromForm] string transactionId)
    {
        var userValid = await _updateBalance.ValidateUser(authUserId);

        if (!userValid)
        {
            return BadRequest("User invalid");
        }

        var updateBalanceRequest = new UpdateBalanceDto
        {
            UserId = authUserId,
            BalanceValue = topUpValue
        };

        UploadedCloudImageResponse uploadedCloudImage;
        ValidationErrors validation;
        if (!transactionId.Equals("0") && file.Length == 0)
        {
            uploadedCloudImage = new UploadedCloudImageResponse
            {
                Name = "",
                Bucket = "",
                MediaLink = "",
                TimeCreated = DateTime.Now,
                Updated = DateTime.Now,
            };

            var response =
                await _updateBalance.RegisterPaymentWithTransactionInput(transactionId, updateBalanceRequest,
                    uploadedCloudImage);

            if (!response.Registered)
            {
                validation = new ValidationErrors
                {
                    Message = new[] { response.Message }
                };
                return BadRequest(new BadRequest(validation));
            }

            return Ok(response);
        }


        if (file.Length == 0 && transactionId.Equals("0"))
        {
            validation = new ValidationErrors
            {
                File = new[] { "Check if file or transaction id are valid" }
            };
            return BadRequest(new BadRequest(validation));
        }


        try
        {
            uploadedCloudImage = await _imageService.Upload(file);
        }
        catch (ApplicationException e)
        {
            validation = new ValidationErrors
            {
                File = new[] { "Processing image failed,please retry" }
            };
            return BadRequest(new BadRequest(validation));
        }


        var responseWithImage =
            await _updateBalance.RegisterPaymentWitTransactionImage(updateBalanceRequest, uploadedCloudImage);
        if (!responseWithImage.Registered)
        {
            await _imageService.Delete(uploadedCloudImage.Bucket, uploadedCloudImage.Name);
            validation = new ValidationErrors
            {
                Message = new[] { responseWithImage.Message }
            };
            return BadRequest(new BadRequest(validation));
        }


        return Ok(responseWithImage);
    }
}