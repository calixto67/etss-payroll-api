using Microsoft.Data.SqlClient;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.DTOs.CompanySettings;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class CompanySettingsService : ICompanySettingsService
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };

    /// <summary>URL path for company logo when stored in GlobalConfig (database).</summary>
    public const string CompanyLogoApiPath = "/api/v1/global-config/company-logo";

    private const string SP = "sp_CompanySettings";

    private readonly ISqlExecutor _sql;
    private readonly IGlobalConfigService _globalConfig;

    public CompanySettingsService(ISqlExecutor sql, IGlobalConfigService globalConfig)
    {
        _sql          = sql;
        _globalConfig = globalConfig;
    }

    // ── Internal row types for Dapper mapping ────────────────────────────

    private sealed class CompanySettingsRow
    {
        public string CompanyName { get; set; } = "";
        public string? Address { get; set; }
        public DateTime? DateStarted { get; set; }
        public string? TaxNo { get; set; }
        public string? BirNo { get; set; }
        public string? EmployerSssNo { get; set; }
        public string? IndustryClassification { get; set; }
        public string? LogoPath { get; set; }
        public string DateFormat { get; set; } = "MM/dd/yyyy";
        public decimal DefaultSssContribution { get; set; }
        public decimal DefaultPhilHealthContribution { get; set; }
        public decimal DefaultPagIbigContribution { get; set; }
    }

    // ── Public methods ───────────────────────────────────────────────────

    public async Task<CompanySettingsDto> GetAsync(CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<CompanySettingsRow>(
                SP, new { ActionType = "GET" }, ct);

            // SP auto-creates singleton if none exists, so row should always be returned
            if (row is null)
                throw new AppException("Company settings not found.");

            var hasDbLogo = await _globalConfig.HasCompanyLogoAsync(ct);
            return ToDto(row, hasDbLogo);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<CompanySettingsDto> UpdateAsync(UpdateCompanySettingsDto dto, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<CompanySettingsRow>(SP, new
            {
                ActionType               = "UPDATE",
                CompanyName              = dto.CompanyName.Trim(),
                Address                  = dto.Address?.Trim(),
                DateStarted              = !string.IsNullOrWhiteSpace(dto.DateStarted) ? DateTime.Parse(dto.DateStarted) : (DateTime?)null,
                TaxNo                    = dto.TaxNo?.Trim(),
                BirNo                    = dto.BirNo?.Trim(),
                EmployerSssNo            = dto.EmployerSssNo?.Trim(),
                IndustryClassification   = dto.IndustryClassification?.Trim(),
                DateFormat               = dto.DateFormat?.Trim(),
                UpdatedBy                = updatedBy,
            }, ct) ?? throw new AppException("Failed to update company settings.");

            var hasDbLogo = await _globalConfig.HasCompanyLogoAsync(ct);
            return ToDto(row, hasDbLogo);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<CompanySettingsDto> UpdateDeductionSettingsAsync(UpdateDeductionSettingsDto dto, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<CompanySettingsRow>(SP, new
            {
                ActionType                       = "UPDATE_DEDUCTIONS",
                DefaultSssContribution           = dto.DefaultSssContribution,
                DefaultPhilHealthContribution    = dto.DefaultPhilHealthContribution,
                DefaultPagIbigContribution       = dto.DefaultPagIbigContribution,
                UpdatedBy                        = updatedBy,
            }, ct) ?? throw new AppException("Failed to update deduction settings.");

            var hasDbLogo = await _globalConfig.HasCompanyLogoAsync(ct);
            return ToDto(row, hasDbLogo);
        }
        catch (SqlException ex) { throw new AppException(ex.Message); }
    }

    public async Task<string> UploadLogoAsync(Stream fileStream, string fileName, string updatedBy, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(fileName);
        if (!AllowedExtensions.Contains(ext))
            throw new InvalidOperationException($"File type '{ext}' is not allowed. Accepted: jpg, jpeg, png, gif, webp, svg.");

        var contentType = ext.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "image/png"
        };

        // Store in GlobalConfig (database)
        await _globalConfig.UploadCompanyLogoAsync(fileStream, contentType, updatedBy, ct);
        return CompanyLogoApiPath;
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    private static CompanySettingsDto ToDto(CompanySettingsRow r, bool useGlobalConfigLogo) => new()
    {
        CompanyName                  = r.CompanyName,
        Address                      = r.Address,
        DateStarted                  = r.DateStarted?.ToString("yyyy-MM-dd"),
        TaxNo                        = r.TaxNo,
        BirNo                        = r.BirNo,
        EmployerSssNo                = r.EmployerSssNo,
        IndustryClassification       = r.IndustryClassification,
        LogoUrl                      = useGlobalConfigLogo ? CompanyLogoApiPath : (r.LogoPath is not null ? $"/{r.LogoPath}" : null),
        DateFormat                   = r.DateFormat,
        DefaultSssContribution       = r.DefaultSssContribution,
        DefaultPhilHealthContribution = r.DefaultPhilHealthContribution,
        DefaultPagIbigContribution   = r.DefaultPagIbigContribution,
    };
}
