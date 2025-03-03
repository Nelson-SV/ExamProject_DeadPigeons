using Common.ErrorMessages;
using Common.SupportedFormat;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Service.Validators;

public class UploadedImageValidator : AbstractValidator<IFormFile>
{
    public UploadedImageValidator()
    {
        RuleFor(x => x.Name).Must(IsFormatValid).WithMessage(ErrorMessages.GetMessage(ErrorCode.FormatNotSupported));
    }

    private bool IsFormatValid(string name)
    {
        string[] format = name.Split(".");
        return SupportedFormats.isPermitedFormat("." + format[1]);
    }

}