using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.InterfacesCustomer;
using Service.TransferModels.Responses;

namespace Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    [Route("history")]
    [Authorize(Roles = Role.Player)]
    public ActionResult<GameTicketsPageResultDto> GetUserTicketsHistory(
        [FromQuery] string userId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 7)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("UserId is required.");
        }

        var pagedResult = _service.GetUserTicketsHistory(userId, page, pageSize);
        return Ok(pagedResult);
    }

    

}