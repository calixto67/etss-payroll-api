namespace PayrollApi.Application.DTOs.Employee;

public sealed class EmployeeDocumentDto
{
    public int      Id           { get; init; }
    public int      EmployeeId   { get; init; }
    public string   DocumentType { get; init; } = string.Empty;
    public string   DocumentName { get; init; } = string.Empty;
    public string   FilePath     { get; init; } = string.Empty;
    public DateTime? ExpiryDate  { get; init; }
    public bool     IsVerified   { get; init; }
    public DateTime CreatedAt    { get; init; }
}

public sealed class UploadDocumentDto
{
    public int      DocumentType { get; init; }
    public string   DocumentName { get; init; } = string.Empty;
    public string   FilePath     { get; init; } = string.Empty;
    public DateTime? ExpiryDate  { get; init; }
}
