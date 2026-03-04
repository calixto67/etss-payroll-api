namespace PayrollApi.Domain.Entities;

public class PayrollPeriod : BaseEntity
{
    public string PeriodCode { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime PayDate { get; set; }
    public PeriodType PeriodType { get; set; }
    public bool IsClosed { get; set; } = false;

    public ICollection<PayrollRecord> PayrollRecords { get; set; } = new List<PayrollRecord>();
}

public enum PeriodType { SemiMonthly = 1, Monthly = 2, Weekly = 3 }
