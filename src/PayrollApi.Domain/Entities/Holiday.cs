namespace PayrollApi.Domain.Entities;

public class Holiday : BaseEntity
{
    public string Name         { get; set; } = string.Empty;
    public DateTime Date       { get; set; }
    public HolidayType Type    { get; set; } = HolidayType.Public;
    public string Region       { get; set; } = string.Empty;
    public bool IsRecurring    { get; set; }
}

public enum HolidayType
{
    Public   = 1,
    Company  = 2,
    Regional = 3,
    Special  = 4
}
