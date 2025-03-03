using System.Text.RegularExpressions;
using Common.ErrorMessages;
using FluentValidation;
using Service.TransferModels.Requests;

namespace Service.Validators;

public class PasswordValidator : AbstractValidator<ResetPasswordDto>
{
    public PasswordValidator()
    {
        RuleFor(x => x.password)
            .NotEmpty()
            .MinimumLength(6).WithMessage(ErrorMessages.GetMessage(ErrorCode.InvalidPassword));
        RuleFor(x => x.password).Must(ValidateUpperCase!).Must(ValidateLowerCase!).Must(ValidateHasNumber!)
            .Must(ValidateSpecialCharacter!).WithMessage(ErrorMessages.GetMessage(ErrorCode.InvalidPassword));
    }


    private  bool ValidateUpperCase(string password)
    {
        return Regex.IsMatch(password, @"[A-Z]");
    }


    private bool ValidateLowerCase(string password)
    {
        return Regex.IsMatch(password, @"[a-z]");
    }

    private bool ValidateHasNumber(string password)
    {
        return Regex.IsMatch(password, @"\d");
    }

    private bool ValidateSpecialCharacter(string password)
    {
        const string specialCharsPattern = @"[!@#$%^&*(),.?\"":{}|<>]";
        return Regex.IsMatch(password, specialCharsPattern);
    }
}