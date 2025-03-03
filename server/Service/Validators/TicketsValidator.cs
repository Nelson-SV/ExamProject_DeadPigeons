using Common.ErrorMessages;
using FluentValidation;
using Service.TransferModels.Requests;

namespace Service.Validators;

public class TicketsValidator : AbstractValidator<CreateTicketDto>
{
    public TicketsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorId));
        
        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorId));

        RuleFor(x => x.Sequence)
            .NotEmpty().WithMessage(ErrorMessages.GetMessage(ErrorCode.ErrorId));


    }
}
