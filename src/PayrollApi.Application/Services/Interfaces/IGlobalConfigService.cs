namespace PayrollApi.Application.Services.Interfaces;

/// <summary>Service for global configuration (e.g. company logo stored in DB).</summary>
public interface IGlobalConfigService
{
    /// <summary>Returns the company logo bytes and content type if present.</summary>
    Task<(byte[] bytes, string contentType)?> GetCompanyLogoAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns true if a company logo is stored.</summary>
    Task<bool> HasCompanyLogoAsync(CancellationToken cancellationToken = default);

    /// <summary>Stores the company logo in the database.</summary>
    Task UploadCompanyLogoAsync(Stream fileStream, string contentType, string updatedBy, CancellationToken cancellationToken = default);
}
