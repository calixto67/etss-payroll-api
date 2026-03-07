namespace PayrollApi.Domain.Entities;

public class EmployeeDocument : BaseEntity
{
    public int          EmployeeId   { get; set; }
    public DocumentType DocumentType { get; set; }
    public string       DocumentName { get; set; } = string.Empty;
    public string       FilePath     { get; set; } = string.Empty;
    public DateTime?    ExpiryDate   { get; set; }
    public bool         IsVerified   { get; set; } = false;

    public Employee? Employee { get; set; }
}

public enum DocumentType
{
    Resume        = 0,
    GovernmentId  = 1,
    Contract      = 2,
    Certificate   = 3,
    MedicalRecord = 4,
    Other         = 99
}
