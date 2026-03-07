using FluentValidation;
using PayrollApi.Application.DTOs.Employee;

namespace PayrollApi.Application.Validators.Employee;

public class ChangeEmployeeStatusValidator : AbstractValidator<ChangeEmployeeStatusDto>
{
    private static readonly int[] TerminalStatuses = [4, 5]; // Terminated, Retired

    public ChangeEmployeeStatusValidator()
    {
        RuleFor(x => x.NewStatus)
            .InclusiveBetween(1, 6).WithMessage("Invalid status value.");

        RuleFor(x => x.Remarks)
            .NotEmpty().WithMessage("Remarks are required when changing status.")
            .MaximumLength(500);

        RuleFor(x => x.LastWorkingDate)
            .NotNull().WithMessage("Last working date is required for Terminated or Retired status.")
            .When(x => TerminalStatuses.Contains(x.NewStatus));

        RuleFor(x => x.LastWorkingDate)
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Last working date cannot be in the future.")
            .When(x => x.LastWorkingDate.HasValue);
    }
}
