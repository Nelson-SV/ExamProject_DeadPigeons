// using System.Text.RegularExpressions;
// using Common.ErrorMessages;
// using FluentValidation;
// using Service.TransferModels.Requests;
//
// namespace Service.Validators;
//
// public class PasswordResetRequestValidator : AbstractValidator<PasswordResetRequest>
// {
//     public PasswordResetRequestValidator()
//     {
//        RuleFor(x => x.Email).NotEmpty();
//        RuleFor(x => x.Token).NotEmpty();
//         RuleFor(x => x.NewPassword).Must(ValidateUpperCase!).Must(ValidateLowerCase!).Must(ValidateHasNumber!)
//             .Must(ValidateSpecialCharacter!).WithMessage(ErrorMessages.GetMessage(ErrorCode.InvalidPassword));
//     }
//     
//     private  bool ValidateUpperCase(string password)
//     {
//         return Regex.IsMatch(password, @"[A-Z]");
//     }
//
//
//     private bool ValidateLowerCase(string password)
//     {
//         return Regex.IsMatch(password, @"[a-z]");
//     }
//
//     private bool ValidateHasNumber(string password)
//     {
//         return Regex.IsMatch(password, @"\d");
//     }
//
//     private bool ValidateSpecialCharacter(string password)
//     {
//         const string specialCharsPattern = @"[!@#$%^&*(),.?\"":{}|<>]";
//         return Regex.IsMatch(password, specialCharsPattern);
//     }
// }