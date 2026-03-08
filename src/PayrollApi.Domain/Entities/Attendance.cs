namespace PayrollApi.Domain.Entities;

public class Attendance : BaseEntity
{
    public int PayrollPeriodId { get; set; }
    public int EmployeeId { get; set; }
    public decimal DaysWorked { get; set; }
    public decimal TotalDays { get; set; }
    public decimal LateHours { get; set; }
    public decimal UndertimeHours { get; set; }
    public decimal OtHours { get; set; }
    public decimal NightDiffHours { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Complete;
    public string? Issue { get; set; }
    public string? ResolutionNotes { get; set; }

    // Navigation properties
    public PayrollPeriod PayrollPeriod { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public ICollection<AttendanceDetail> Details { get; set; } = new List<AttendanceDetail>();
}

public class AttendanceDetail : BaseEntity
{
    public int AttendanceId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? TimeIn { get; set; }
    public TimeSpan? TimeOut { get; set; }
    public decimal LateHours { get; set; }
    public decimal UndertimeHours { get; set; }
    public decimal OtHours { get; set; }
    public decimal NightDiffHours { get; set; }
    public string Status { get; set; } = "Present";
    public string? Remarks { get; set; }

    // Navigation
    public Attendance Attendance { get; set; } = null!;
}

public enum AttendanceStatus { Complete = 1, Review = 2, Absent = 3 }
