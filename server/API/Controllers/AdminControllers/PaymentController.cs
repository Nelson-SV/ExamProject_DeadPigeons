using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.AdminService.Payment;
using Service.TransferModels.Requests.Admin;
using Service.TransferModels.Responses;

namespace Api.Controllers.AdminControllers;

[ApiController]
[Route("api/adminPayment")]

public class PaymentController : ControllerBase
{
    private readonly IPaymentService _service;


    public PaymentController(IPaymentService service)
    {
        _service = service;
    }

    [HttpGet]
    [Route("pendingPayments")]
    [Authorize(Roles = Role.Admin)]
    public ActionResult<PaymentPageResultDto<PaymentDto>> GetUserPendingPayments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        var pagedResult = _service.GetPendingPayments(page, pageSize);
        return Ok(pagedResult);
       
    }

    [HttpPost]
    [Route("updatePendingPayments")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult> UpdatePendingPayments([FromBody] UpdatePaymentDto updatePaymentDto)
    {
       
        await _service.UpdatePendingPayments(updatePaymentDto);
        return Ok();
        
    }

    [HttpPost]
    [Route("declinePendingPayments")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult> DeclinePendingPayments([FromBody] DeclinePaymentDto declinePaymentDto)
    {

        await _service.DeclinePendingPayments( declinePaymentDto);
        return Ok();
       
    }



}
