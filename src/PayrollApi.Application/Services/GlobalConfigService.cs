using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class GlobalConfigService : IGlobalConfigService
{
    public const string CompanyLogoKey = "CompanyLogo";

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml"
    };

    private const string SP = "sp_GlobalConfig";

    private readonly ISqlExecutor _sql;

    public GlobalConfigService(ISqlExecutor sql) => _sql = sql;

    // ── Internal row types for Dapper mapping ────────────────────────────

    private sealed class GlobalConfigRow
    {
        public string ConfigName { get; set; } = "";
        public byte[]? ConfigValue { get; set; }
        public string? MimeType { get; set; }
    }

    // ── Public methods ───────────────────────────────────────────────────

    public async Task<(byte[] bytes, string contentType)?> GetCompanyLogoAsync(CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<GlobalConfigRow>(
                SP, new { ActionType = "GET_BY_NAME", ConfigName = CompanyLogoKey }, ct);

            if (row is null || row.ConfigValue is null || row.ConfigValue.Length == 0)
                return null;

            return (row.ConfigValue, row.MimeType ?? "image/png");
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<bool> HasCompanyLogoAsync(CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<GlobalConfigRow>(
                SP, new { ActionType = "GET_BY_NAME", ConfigName = CompanyLogoKey }, ct);

            return row is not null && row.ConfigValue is { Length: > 0 };
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task UploadCompanyLogoAsync(Stream fileStream, string contentType, string updatedBy, CancellationToken ct = default)
    {
        if (!AllowedContentTypes.Contains(contentType))
            throw new InvalidOperationException($"Content type '{contentType}' is not allowed. Accepted: image/jpeg, image/png, image/gif, image/webp, image/svg+xml.");

        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms, ct);
        var bytes = ms.ToArray();
        if (bytes.Length == 0)
            throw new InvalidOperationException("File is empty.");
        const int maxBytes = 2 * 1024 * 1024; // 2 MB
        if (bytes.Length > maxBytes)
            throw new InvalidOperationException($"File size must not exceed 2 MB (got {bytes.Length / 1024} KB).");

        try
        {
            await _sql.ExecuteAsync(SP, new
            {
                ActionType  = "UPSERT",
                ConfigName  = CompanyLogoKey,
                ConfigValue = bytes,
                MimeType    = contentType,
                UpdatedBy   = updatedBy,
            }, ct);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }
}
