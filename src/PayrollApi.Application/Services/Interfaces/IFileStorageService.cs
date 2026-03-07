namespace PayrollApi.Application.Services.Interfaces;

/// <summary>
/// Abstracts file persistence so the Application layer stays infrastructure-agnostic.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves a stream to a relative path under the static-files root.
    /// Returns the relative URL path (e.g., "uploads/logo/company-logo.png").
    /// </summary>
    Task<string> SaveAsync(Stream stream, string relativePath, CancellationToken cancellationToken = default);
}
