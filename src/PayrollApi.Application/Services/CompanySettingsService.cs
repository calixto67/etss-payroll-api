using PayrollApi.Application.DTOs.CompanySettings;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class CompanySettingsService : ICompanySettingsService
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };

    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _storage;

    public CompanySettingsService(IUnitOfWork uow, IFileStorageService storage)
    {
        _uow     = uow;
        _storage = storage;
    }

    public async Task<CompanySettingsDto> GetAsync(CancellationToken ct = default)
    {
        var settings = await EnsureSingletonAsync(ct);
        return ToDto(settings);
    }

    public async Task<CompanySettingsDto> UpdateAsync(UpdateCompanySettingsDto dto, string updatedBy, CancellationToken ct = default)
    {
        var settings = await EnsureSingletonAsync(ct);
        settings.CompanyName = dto.CompanyName.Trim();
        settings.UpdatedAt   = DateTime.UtcNow;
        settings.UpdatedBy   = updatedBy;
        await _uow.CompanySettings.UpdateAsync(settings, ct);
        await _uow.CommitAsync(ct);
        return ToDto(settings);
    }

    public async Task<string> UploadLogoAsync(Stream fileStream, string fileName, string updatedBy, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(fileName);
        if (!AllowedExtensions.Contains(ext))
            throw new InvalidOperationException($"File type '{ext}' is not allowed. Accepted: jpg, jpeg, png, gif, webp, svg.");

        // Use a fixed filename so the URL stays stable across uploads
        var relativePath = $"uploads/logo/company-logo{ext}";
        await _storage.SaveAsync(fileStream, relativePath, ct);

        var settings = await EnsureSingletonAsync(ct);
        settings.LogoPath  = relativePath;
        settings.UpdatedAt = DateTime.UtcNow;
        settings.UpdatedBy = updatedBy;
        await _uow.CompanySettings.UpdateAsync(settings, ct);
        await _uow.CommitAsync(ct);

        return relativePath;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<CompanySettings> EnsureSingletonAsync(CancellationToken ct)
    {
        var existing = await _uow.CompanySettings.GetSingletonAsync(ct);
        if (existing is not null) return existing;

        var created = new CompanySettings { CompanyName = "My Company", CreatedBy = "system" };
        await _uow.CompanySettings.AddAsync(created, ct);
        await _uow.CommitAsync(ct);
        return created;
    }

    private static CompanySettingsDto ToDto(CompanySettings s) => new()
    {
        CompanyName = s.CompanyName,
        LogoUrl     = s.LogoPath is not null ? $"/{s.LogoPath}" : null,
    };
}
