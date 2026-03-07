namespace PayrollApi.Domain.Entities;

/// <summary>
/// A named permission role that can be assigned to users.
/// Each role carries a set of per-module permissions (see RolePermission).
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
