using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    public FileStorageService(IWebHostEnvironment env) => _env = env;

    public async Task<string> SaveAsync(Stream stream, string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_env.WebRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var fs = File.Create(fullPath);
        await stream.CopyToAsync(fs, cancellationToken);

        return relativePath;
    }
}
