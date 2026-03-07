using FluentValidation;
using PayrollApi.Application.DTOs.Employee;

namespace PayrollApi.Application.Validators.Employee;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeValidator()
    {
        // Identity
        RuleFor(x => x.EmployeeCode)
            .NotEmpty().WithMessage("Employee code is required.")
            .MaximumLength(20).WithMessage("Employee code must not exceed 20 characters.")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Employee code must be alphanumeric uppercase.");

        // Personal
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.MiddleName).MaximumLength(100).When(x => x.MiddleName is not null);
        RuleFor(x => x.Suffix).MaximumLength(20).When(x => x.Suffix is not null);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Today.AddYears(-18)).WithMessage("Employee must be at least 18 years old.")
            .GreaterThan(DateTime.Today.AddYears(-80)).WithMessage("Invalid date of birth.");

        RuleFor(x => x.Gender)
            .InclusiveBetween(0, 3).WithMessage("Invalid gender value.");

        RuleFor(x => x.MaritalStatus)
            .InclusiveBetween(0, 3).WithMessage("Invalid marital status value.");

        // Government IDs
        RuleFor(x => x.TaxIdentificationNumber)
            .NotEmpty().WithMessage("TIN is required.")
            .Matches(@"^\d{9,12}$").WithMessage("TIN must be 9–12 digits.");

        RuleFor(x => x.SssNumber)
            .NotEmpty().WithMessage("SSS number is required.")
            .MaximumLength(30);

        RuleFor(x => x.PhilHealthNumber)
            .NotEmpty().WithMessage("PhilHealth number is required.")
            .MaximumLength(30);

        RuleFor(x => x.PagIbigNumber)
            .NotEmpty().WithMessage("Pag-IBIG number is required.")
            .MaximumLength(30);

        // Contact
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Work email is required.")
            .EmailAddress().WithMessage("A valid work email address is required.")
            .MaximumLength(256);

        RuleFor(x => x.PersonalEmail)
            .EmailAddress().WithMessage("A valid personal email address is required.")
            .When(x => !string.IsNullOrWhiteSpace(x.PersonalEmail));

        RuleFor(x => x.MobileNumber)
            .NotEmpty().WithMessage("Mobile number is required.")
            .MaximumLength(20);

        // Present Address
        RuleFor(x => x.PresentAddress).NotEmpty().WithMessage("Present address is required.").MaximumLength(500);
        RuleFor(x => x.PresentCity).NotEmpty().WithMessage("Present city is required.").MaximumLength(100);
        RuleFor(x => x.PresentProvince).NotEmpty().WithMessage("Present province is required.").MaximumLength(100);
        RuleFor(x => x.PresentZipCode).NotEmpty().WithMessage("Present zip code is required.").MaximumLength(10);

        // Permanent Address — required only when not same as present
        When(x => !x.SameAsPresentAddress, () =>
        {
            RuleFor(x => x.PermanentAddress).NotEmpty().WithMessage("Permanent address is required.").MaximumLength(500);
            RuleFor(x => x.PermanentCity).NotEmpty().WithMessage("Permanent city is required.").MaximumLength(100);
            RuleFor(x => x.PermanentProvince).NotEmpty().WithMessage("Permanent province is required.").MaximumLength(100);
            RuleFor(x => x.PermanentZipCode).NotEmpty().WithMessage("Permanent zip code is required.").MaximumLength(10);
        });

        // Employment
        RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("A valid department is required.");
        RuleFor(x => x.PositionId).GreaterThan(0).WithMessage("A valid position is required.");
        RuleFor(x => x.ManagerId).GreaterThan(0).When(x => x.ManagerId.HasValue).WithMessage("Invalid manager.");

        RuleFor(x => x.EmploymentType)
            .InclusiveBetween(1, 4).WithMessage("Invalid employment type.");

        // Dates
        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("Hire date is required.")
            .LessThanOrEqualTo(DateTime.Today.AddDays(30)).WithMessage("Hire date cannot be more than 30 days in the future.");

        RuleFor(x => x.SalaryEffectiveDate)
            .NotEmpty().WithMessage("Salary effective date is required.")
            .LessThanOrEqualTo(x => x.HireDate).WithMessage("Salary effective date must be on or before hire date.");

        RuleFor(x => x.ProbationEndDate)
            .GreaterThan(x => x.HireDate).WithMessage("Probation end date must be after hire date.")
            .When(x => x.ProbationEndDate.HasValue);

        When(x => x.EmploymentType == 4 /* Probationary */, () =>
        {
            RuleFor(x => x.ProbationEndDate)
                .NotNull().WithMessage("Probation end date is required for probationary employees.");
        });

        // Compensation
        RuleFor(x => x.BasicSalary).GreaterThan(0).WithMessage("Basic salary must be greater than zero.");
        RuleFor(x => x.SalaryFrequency).InclusiveBetween(0, 4).WithMessage("Invalid salary frequency.");
        RuleFor(x => x.BankAccountNumber).NotEmpty().WithMessage("Bank account number is required.").MaximumLength(64);
        RuleFor(x => x.BankName).NotEmpty().WithMessage("Bank name is required.").MaximumLength(128);
    }
}
