namespace PayrollApi.Domain.Entities;

/// <summary>
/// Key-value store for global configuration (e.g. company logo image stored as binary).
/// No soft-delete; uses ConfigName and ConfigValue.
/// </summary>
public class GlobalConfig
{
    public int Id { get; set; }

    /// <summary>Unique config name, e.g. "CompanyLogo".</summary>
    public string ConfigName { get; set; } = string.Empty;

    /// <summary>Binary or string value (e.g. image bytes).</summary>
    public byte[] ConfigValue { get; set; } = Array.Empty<byte>();

    /// <summary>MIME type when value is binary content, e.g. "image/png".</summary>
    public string? MimeType { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; } = "system";
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
}
