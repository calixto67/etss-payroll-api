using FluentValidation;
using PayrollApi.Application.DTOs.Employee;

namespace PayrollApi.Application.Validators.Employee;

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(64);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(64);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.BasicSalary)
            .GreaterThan(0).WithMessage("Basic salary must be greater than zero.");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("A valid department is required.");

        RuleFor(x => x.PositionId)
            .GreaterThan(0).WithMessage("A valid position is required.");
    }
}
