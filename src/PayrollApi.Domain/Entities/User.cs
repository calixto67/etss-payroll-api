namespace PayrollApi.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    /// <summary>Legacy string role used for JWT claims (Admin, PayrollAdmin, HrStaff, Manager).</summary>
    public string Role { get; set; } = "HrStaff";

    /// <summary>Optional link to a granular permission Role. Admins bypass role permission checks.</summary>
    public int? RoleId { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public int? EmployeeId { get; set; }

    public Employee? Employee { get; set; }
    public Role? PermissionRole { get; set; }
}
