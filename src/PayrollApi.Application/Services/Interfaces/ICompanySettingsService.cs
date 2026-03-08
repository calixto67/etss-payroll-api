using PayrollApi.Application.DTOs.CompanySettings;

namespace PayrollApi.Application.Services.Interfaces;

public interface ICompanySettingsService
{
    Task<CompanySettingsDto> GetAsync(CancellationToken cancellationToken = default);
    Task<CompanySettingsDto> UpdateAsync(UpdateCompanySettingsDto dto, string updatedBy, CancellationToken cancellationToken = default);

    /// <summary>Saves the uploaded logo file and returns the public URL.</summary>
    Task<string> UploadLogoAsync(Stream fileStream, string fileName, string updatedBy, CancellationToken cancellationToken = default);

    Task<CompanySettingsDto> UpdateDeductionSettingsAsync(UpdateDeductionSettingsDto dto, string updatedBy, CancellationToken cancellationToken = default);
}
