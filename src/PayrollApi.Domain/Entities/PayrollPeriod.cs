namespace PayrollApi.Domain.Entities;

public class PayrollPeriod : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string PeriodCode { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime PayDate { get; set; }
    public PeriodType PeriodType { get; set; }
    public PeriodStatus Status { get; set; } = PeriodStatus.Draft;
    public bool IsClosed { get; set; } = false;

    public ICollection<PayrollRecord> PayrollRecords { get; set; } = new List<PayrollRecord>();
}

public enum PeriodType   { SemiMonthly = 1, Monthly = 2, Weekly = 3 }
public enum PeriodStatus { Draft = 1, Open = 2, Computed = 3, Approved = 4, Locked = 5, Exported = 6 }
