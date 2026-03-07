using FluentValidation;
using PayrollApi.Application.DTOs.Employee;

namespace PayrollApi.Application.Validators.Employee;

public class CreateEmergencyContactValidator : AbstractValidator<CreateEmergencyContactDto>
{
    public CreateEmergencyContactValidator()
    {
        RuleFor(x => x.ContactName)
            .NotEmpty().WithMessage("Contact name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Relationship)
            .NotEmpty().WithMessage("Relationship is required.")
            .MaximumLength(50);

        RuleFor(x => x.MobileNumber)
            .NotEmpty().WithMessage("Mobile number is required.")
            .MaximumLength(20);
    }
}

public class UpdateEmergencyContactValidator : AbstractValidator<UpdateEmergencyContactDto>
{
    public UpdateEmergencyContactValidator()
    {
        RuleFor(x => x.ContactName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Relationship).NotEmpty().MaximumLength(50);
        RuleFor(x => x.MobileNumber).NotEmpty().MaximumLength(20);
    }
}
