using FluentValidation;
using PayrollApi.Application.DTOs.Employee;

namespace PayrollApi.Application.Validators.Employee;

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeValidator()
    {
        // Personal
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.").MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.").MaximumLength(100);
        RuleFor(x => x.MiddleName).MaximumLength(100).When(x => x.MiddleName is not null);
        RuleFor(x => x.Suffix).MaximumLength(20).When(x => x.Suffix is not null);
        RuleFor(x => x.Gender).InclusiveBetween(0, 3).WithMessage("Invalid gender value.");
        RuleFor(x => x.MaritalStatus).InclusiveBetween(0, 3).WithMessage("Invalid marital status value.");

        // Government IDs
        RuleFor(x => x.TaxIdentificationNumber)
            .NotEmpty().WithMessage("TIN is required.")
            .Matches(@"^\d{9,12}$").WithMessage("TIN must be 9–12 digits.");
        RuleFor(x => x.SssNumber).NotEmpty().WithMessage("SSS number is required.").MaximumLength(30);
        RuleFor(x => x.PhilHealthNumber).NotEmpty().WithMessage("PhilHealth number is required.").MaximumLength(30);
        RuleFor(x => x.PagIbigNumber).NotEmpty().WithMessage("Pag-IBIG number is required.").MaximumLength(30);

        // Contact
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Work email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256);

        RuleFor(x => x.PersonalEmail)
            .EmailAddress().WithMessage("A valid personal email is required.")
            .When(x => !string.IsNullOrWhiteSpace(x.PersonalEmail));

        RuleFor(x => x.MobileNumber).NotEmpty().WithMessage("Mobile number is required.").MaximumLength(20);

        // Present Address
        RuleFor(x => x.PresentAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.PresentCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PresentProvince).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PresentZipCode).NotEmpty().MaximumLength(10);

        When(x => !x.SameAsPresentAddress, () =>
        {
            RuleFor(x => x.PermanentAddress).NotEmpty().MaximumLength(500);
            RuleFor(x => x.PermanentCity).NotEmpty().MaximumLength(100);
            RuleFor(x => x.PermanentProvince).NotEmpty().MaximumLength(100);
            RuleFor(x => x.PermanentZipCode).NotEmpty().MaximumLength(10);
        });

        // Employment
        RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("A valid department is required.");
        RuleFor(x => x.PositionId).GreaterThan(0).WithMessage("A valid position is required.");
        RuleFor(x => x.ManagerId).GreaterThan(0).When(x => x.ManagerId.HasValue);
        RuleFor(x => x.EmploymentType).InclusiveBetween(1, 4);

        // Compensation
        RuleFor(x => x.BasicSalary).GreaterThan(0).WithMessage("Basic salary must be greater than zero.");
        RuleFor(x => x.SalaryFrequency).InclusiveBetween(0, 4);
        RuleFor(x => x.SalaryEffectiveDate)
            .NotEmpty().WithMessage("Salary effective date is required.")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Salary effective date cannot be in the past.");
        RuleFor(x => x.BankAccountNumber).NotEmpty().MaximumLength(64);
        RuleFor(x => x.BankName).NotEmpty().MaximumLength(128);
    }
}
