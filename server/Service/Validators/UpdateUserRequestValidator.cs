using FluentValidation;
using Service.TransferModels.Requests.Admin;

namespace Service.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty().Length(8);
        RuleFor(x => x.IsActive).NotEmpty();
    }
}