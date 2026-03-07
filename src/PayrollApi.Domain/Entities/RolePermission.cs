namespace PayrollApi.Domain.Entities;

/// <summary>
/// Stores CRUD permission flags for a specific application module scoped to a Role.
/// ModuleKey matches the route-id used in the Angular sidemenu (e.g., "attendance", "payroll-run").
/// </summary>
public class RolePermission : BaseEntity
{
    public int RoleId { get; set; }
    public string ModuleKey { get; set; } = string.Empty;

    public bool CanView   { get; set; } = false;
    public bool CanAdd    { get; set; } = false;
    public bool CanUpdate { get; set; } = false;
    public bool CanDelete { get; set; } = false;

    public Role Role { get; set; } = null!;
}
