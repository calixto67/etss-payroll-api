using FluentValidation;
using PayrollApi.Application.DTOs.Auth;

namespace PayrollApi.Application.Validators.Auth;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
        { "Admin", "PayrollAdmin", "HrStaff", "Manager" };

    public CreateUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(2).WithMessage("Username must be at least 2 characters.")
            .MaximumLength(80).WithMessage("Username must not exceed 80 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.Role)
            .Must(r => AllowedRoles.Contains(r))
            .WithMessage("Role must be one of: Admin, PayrollAdmin, HrStaff, Manager.");
    }
}
