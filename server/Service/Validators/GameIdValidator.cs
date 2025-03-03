using Common.ErrorMessages;
using FluentValidation;
using Service.TransferModels.Responses;

namespace Service.Validators;

public class GameIdValidator:AbstractValidator<GameIdDto>
{
    public GameIdValidator()
    {
        RuleFor((x) => x.Guid).NotNull().NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCode.GameIdInvalid));
    }
}