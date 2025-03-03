using Common.BalanceValues;
using Common.ErrorMessages;
using FluentValidation;
using Service.TransferModels.Requests;

namespace Service.Validators;

public class BalanceValidator : AbstractValidator<UpdateBalanceDto>
{
    public BalanceValidator()
    {
        RuleFor(x => x.BalanceValue).NotEmpty().Must(IsBalanceValid)
            .WithMessage(x =>
                $"{x.BalanceValue} is invalid. {ErrorMessages.GetMessage(ErrorCode.BalanceInvalid)} Valid values are: {AppConstants.GetValidValuesString()}");
        RuleFor(x => x.UserId).NotNull().NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCode.UserId));
    }

    private bool IsBalanceValid(int value)
    {
        return AppConstants.TopUpValuesSet.Contains(value);
    }
}