using FluentValidation;
using PayrollApi.Application.DTOs.Employee;

namespace PayrollApi.Application.Validators.Employee;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.EmployeeCode)
            .NotEmpty().WithMessage("Employee code is required.")
            .MaximumLength(20).WithMessage("Employee code must not exceed 20 characters.")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Employee code must be alphanumeric uppercase.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(64).WithMessage("First name must not exceed 64 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(64).WithMessage("Last name must not exceed 64 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Today.AddYears(-18)).WithMessage("Employee must be at least 18 years old.");

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("Hire date is required.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Hire date cannot be in the future.");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("A valid department is required.");

        RuleFor(x => x.PositionId)
            .GreaterThan(0).WithMessage("A valid position is required.");

        RuleFor(x => x.BasicSalary)
            .GreaterThan(0).WithMessage("Basic salary must be greater than zero.");

        RuleFor(x => x.TaxIdentificationNumber)
            .NotEmpty().WithMessage("TIN is required.")
            .Matches(@"^\d{9,12}$").WithMessage("TIN must be 9-12 digits.");

        RuleFor(x => x.SssNumber)
            .NotEmpty().WithMessage("SSS number is required.");

        RuleFor(x => x.PhilHealthNumber)
            .NotEmpty().WithMessage("PhilHealth number is required.");

        RuleFor(x => x.PagIbigNumber)
            .NotEmpty().WithMessage("Pag-IBIG number is required.");
    }
}
