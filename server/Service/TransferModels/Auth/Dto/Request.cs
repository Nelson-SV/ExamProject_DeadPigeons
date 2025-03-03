using FluentValidation;

namespace Service.TransferModels.Auth.Dto;

public record RegisterRequest(string Email, string Name, string Role, string PhoneNumber);

public record LoginRequest(string Email, string Password);

public record InitPasswordResetRequest(string Email);

public class InitPasswordResetRequestValidator : AbstractValidator<InitPasswordResetRequest>
{
    public InitPasswordResetRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

