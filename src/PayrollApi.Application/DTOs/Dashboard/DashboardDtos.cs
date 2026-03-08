namespace PayrollApi.Application.DTOs.Dashboard;

public record DashboardSummaryDto
{
    public EmployeeCountsDto EmployeeCounts { get; init; } = new();
    public ActivePeriodDto? ActivePeriod { get; init; }
    public List<RecentPeriodDto> RecentPeriods { get; init; } = [];
    public PendingTasksDto PendingTasks { get; init; } = new();
    public List<DepartmentBreakdownDto> DepartmentBreakdown { get; init; } = [];
}

public record EmployeeCountsDto
{
    public int TotalEmployees { get; init; }
    public int ActiveEmployees { get; init; }
    public int ProbationaryEmployees { get; init; }
    public int OnLeaveEmployees { get; init; }
    public int SuspendedEmployees { get; init; }
    public int ResignedEmployees { get; init; }
    public int TotalDepartments { get; init; }
    public int TotalBranches { get; init; }
}

public record ActivePeriodDto
{
    public int PeriodId { get; init; }
    public string PeriodName { get; init; } = string.Empty;
    public string PeriodCode { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public DateTime PayDate { get; init; }
    public string PeriodStatus { get; init; } = string.Empty;
    public bool IsClosed { get; init; }
    public int DaysUntilCutoff { get; init; }
    public int PayrollRecordCount { get; init; }
    public decimal TotalGrossPay { get; init; }
    public decimal TotalNetPay { get; init; }
}

public record RecentPeriodDto
{
    public int PeriodId { get; init; }
    public string PeriodName { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string PeriodStatus { get; init; } = string.Empty;
    public int PayrollRecordCount { get; init; }
    public decimal TotalGrossPay { get; init; }
    public decimal TotalNetPay { get; init; }
}

public record PendingTasksDto
{
    public int PendingLeaveApplications { get; init; }
    public int UnresolvedAttendanceIssues { get; init; }
    public int OpenPayPeriods { get; init; }
    public int DraftPayrollRecords { get; init; }
}

public record DepartmentBreakdownDto
{
    public int DepartmentId { get; init; }
    public string DepartmentName { get; init; } = string.Empty;
    public int EmployeeCount { get; init; }
}
