using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.TransferModels.Responses;

namespace Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminUserManagementController : ControllerBase
{
    private readonly IAdminUserManagementService _service;
    private readonly ILogger<AdminUserManagementController> _logger;


    public AdminUserManagementController(IAdminUserManagementService service, ILogger<AdminUserManagementController> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    [HttpGet]
    [Route("users-management")]
    [Authorize(Roles = Role.Admin)]
    public ActionResult<UsersDetailsPageResultDto> GetUsersDetails(
        [FromQuery] string adminId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 7)
    {
        if (string.IsNullOrEmpty(adminId))
        {
            return BadRequest("Admin Id is required.");
        }

        var pagedResult = _service.GetUsersDetails(adminId, page, pageSize);
        return Ok(pagedResult);
    }
    
    [HttpDelete]
    [Route("delete-user")]
    [Authorize(Roles = Role.Admin)]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("User ID is required.");
        }

        try
        {
            await _service.DeleteUserAsync(userId);
            return Ok(new { Message = "User deleted successfully." });
        }
        catch (ApplicationException e)
        {
            _logger.Log(LogLevel.Warning, e.Message, e);
            return BadRequest("Operation failed, try again later.");
        }
    }

}