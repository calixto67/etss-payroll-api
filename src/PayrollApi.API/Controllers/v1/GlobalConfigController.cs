using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/global-config")]
[Produces("application/json")]
public class GlobalConfigController : BaseController
{
    private readonly IGlobalConfigService _globalConfig;

    public GlobalConfigController(IGlobalConfigService globalConfig) => _globalConfig = globalConfig;

    /// <summary>Get company logo image. Returns 404 if no logo has been uploaded. Anonymous so img src can load.</summary>
    [HttpGet("company-logo")]
    [AllowAnonymous]
    [Produces("image/png", "image/jpeg", "image/gif", "image/webp", "image/svg+xml")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompanyLogo(CancellationToken ct)
    {
        var result = await _globalConfig.GetCompanyLogoAsync(ct);
        if (result is null)
            return NotFound();

        var (bytes, contentType) = result.Value;
        return File(bytes, contentType, "company-logo");
    }

    /// <summary>Upload company logo. Admin only. Image is stored in the database.</summary>
    [HttpPost("company-logo")]
    [Authorize(Policy = "PayrollAdmin")]
    [RequestSizeLimit(2 * 1024 * 1024)] // 2 MB
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadCompanyLogo(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { success = false, message = "No file was uploaded." });

        try
        {
            await using var stream = file.OpenReadStream();
            var contentType = file.ContentType ?? "image/png";
            await _globalConfig.UploadCompanyLogoAsync(stream, contentType, CurrentUser, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
