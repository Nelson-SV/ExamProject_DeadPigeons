using System.Security.Authentication;
using API.Responses;
using Common.ErrorMessages;
using DataAccess.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Service;
using Service.AdminService;
using Service.Auth.Dto;
using Service.Security;
using Service.TransferModels.Auth.Dto;
using Service.TransferModels.Requests;
using Service.TransferModels.Requests.Admin;
using Service.TransferModels.Responses.BalanceDto;
using Service.TransferModels.Responses.UserDetails;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    
    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromServices] UserManager<User> userManager,
        [FromServices] IValidator<LoginRequest> validator,
        [FromServices] ITokenClaimsService tokenClaimsService,
        [FromServices] ILogger<AuthController> logger,
        [FromBody] LoginRequest data
    )
    {
    
        var user = await userManager.FindByEmailAsync(data.Email);

        if (user == null || !await userManager.CheckPasswordAsync(user, data.Password.Trim()))
        {
            logger.LogWarning("Invalid login attempt for email: {Email}", data.Email);

            ValidationErrors validation = new ValidationErrors
            {
                Message = new[] { ErrorMessages.GetMessage(ErrorCode.InvalidLogin) }
            };
            return BadRequest(new BadRequest(validation));
        }


        var token = await tokenClaimsService.GetTokenAsync(data.Email);
        logger.LogInformation("Login successful for email: {Email}", data.Email);
        return new LoginResponse(token);
    }

    [HttpPost]
    [Route("create-user")]
    [Authorize(Roles = Role.Admin)]

    public async Task<ActionResult<UsersDetailsDto>> Register(
        IOptions<AppOptions> options,
        [FromServices] UserManager<User> userManager,
        [FromServices] IValidator<RegisterRequest> validator,
        [FromBody] RegisterRequest data,
        [FromServices] IAdminUserManagementService service
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(data.Role) || !Role.All.Any(role =>
                    string.Equals(role, data.Role, StringComparison.OrdinalIgnoreCase)))
            {
                ValidationErrors validation = new ValidationErrors
                {
                    Message = new[] { ErrorMessages.GetMessage(ErrorCode.InvalidRole) }
                };
                return BadRequest(new BadRequest(validation));
            }


            var user = new User { UserName = data.Name, Email = data.Email, PhoneNumber = data.PhoneNumber, EmailConfirmed = true};
            var result = await userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                var validationErrors = result.Errors.Select(e => e.Description).ToArray();
                ValidationErrors validation = new ValidationErrors
                {
                    Message = validationErrors
                };
                return BadRequest(new BadRequest(validation));
            }


            await userManager.AddToRoleAsync(user, data.Role);

            var userProfile = new CreateUserProfileDto
            {
                UserId = user.Id,
                Name = data.Name,
                IsActive = true
            };
            
            var usersDetailsDto = await service.AddUserProfile(userProfile);
            if (usersDetailsDto.UserId != userProfile.UserId)
            {
                // Rollback AspNetUser creation if UserProfile creation fails
                await userManager.DeleteAsync(user);
                return StatusCode(500, new [] { ErrorMessages.GetMessage(ErrorCode.InvalidUserProfile) });
            }
            return Ok(usersDetailsDto);
        }
        catch (Exception ex)
        {
            ValidationErrors validation = new ValidationErrors
            {
                Message = new[] { ErrorMessages.GetMessage(ErrorCode.UnexpectedError), ex.Message }
            };
            return StatusCode(500, new BadRequest(validation));
        }
    }
    
    [HttpPost]
    [Route("init-password-reset")]
    [AllowAnonymous]
    public async Task<IResult> InitPasswordReset(
        IOptions<AppOptions> options,
        [FromServices] UserManager<User> userManager,
        [FromServices] EmailService emailSender,
        [FromBody] InitPasswordResetRequest data,
        [FromServices] ILogger<AuthController> logger,
        [FromServices] IAdminUserManagementService service
    )
    {
        var user = await userManager.FindByEmailAsync(data.Email);
        if (user is not { EmailConfirmed: true }) return Results.Ok();
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var qs = new Dictionary<string, string?> { { "token", token }, { "email", user.Email } };
        var confirmationLink = new UriBuilder(options.Value.AddressPassword)
        {
            Path = "/password-reset",
            Query = QueryString.Create(qs).Value
        }.Uri.ToString();
        try
        {
            await service.SendResetEmail(user.UserName!, user.Email!, confirmationLink);
            return Results.Ok();
        }
        catch (ApplicationException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPut]
    [Route("update-user")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult<UsersDetailsDto>> UpdateUser(
        [FromServices] IValidator<UpdateUserDto> validator,
        [FromServices] UserManager<User> userManager,
        [FromBody] UpdateUserDto request,
        [FromServices] IAdminUserManagementService service
        )
    {
        if (request.UserId.Length == 0)
        {
            return BadRequest(ErrorMessages.GetMessage(ErrorCode.ErrorId));
        }

        try
        {
            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
    
            if (!string.IsNullOrEmpty(request.Name))
            {
                user.UserName = request.Name;
            }
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                user.Email = request.Email;
            }
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                user.PhoneNumber = request.PhoneNumber;
            }

    
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return StatusCode(500, new [] { ErrorMessages.GetMessage(ErrorCode.InternalServerError) });
            }
            
            var userProfile = await service.UpdateUserProfile(request);
            return Ok(userProfile);
        }
        catch(ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }



    [HttpPost]
    [Route("logout")]
    public async Task<IResult> Logout([FromServices] SignInManager<User> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }

    [HttpGet]
    [Route("userinfo")]
    public async Task<AuthUserInfo> UserInfo([FromServices] UserManager<User> userManager,
        [FromServices] IUpdateBalance balance)
    {
        var username = HttpContext.User.Identity?.Name ?? throw new AuthenticationException();
        var user = await userManager.FindByNameAsync(username) ?? throw new AuthenticationException();
        var roles = await userManager.GetRolesAsync(user);


        var isAdmin = roles.Contains(Role.Admin);
        var canPlay = roles.Contains(Role.Player) || isAdmin;
        CurrentBalanceDto balanceValue = new CurrentBalanceDto();
        if (!isAdmin)
        {
            balanceValue = await balance.RetrieveBalance(user.Id);
        }

        var userId = user.Id;
        return new AuthUserInfo(username, isAdmin, canPlay, balanceValue.BalanceValue, userId);
    }



    [HttpPost]
    [Route("password-reset")]
    [AllowAnonymous]
    public async Task<ActionResult> PasswordReset(
        IOptions<AppOptions> options,
        [FromServices] UserManager<User> userManager,
        [FromBody] PasswordResetRequest data
    )
    {
        
        
        ValidationErrors validation;
        var user = await userManager.FindByEmailAsync(data.Email);
        if (user == null)
        {
            validation = new ValidationErrors
            {
                Message = new[] { ErrorMessages.GetMessage(ErrorCode.ErrorEmail) }
            };
            return BadRequest(new BadRequest(validation));
         
        }

        if (string.IsNullOrWhiteSpace(data.Token))
        {
            validation = new ValidationErrors
            {
                Message = new[] { ErrorMessages.GetMessage(ErrorCode.UnexpectedError) }
            };
            return BadRequest(new BadRequest(validation));
        }

        var result = await userManager.ResetPasswordAsync(user!, data.Token, data.Password);

        if (!result.Succeeded)
        {
            validation = new ValidationErrors
            {
                Message = new[] { ErrorMessages.GetMessage(ErrorCode.UnauthorizedAccess) }
            };
            return BadRequest(new BadRequest(validation));
        }

        return Created("", result);
    }
}