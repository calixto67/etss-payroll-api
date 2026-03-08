using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.CompanySettings;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/company-settings")]
public class CompanySettingsController : BaseController
{
    private readonly ICompanySettingsService _settingsService;

    public CompanySettingsController(ICompanySettingsService settingsService) =>
        _settingsService = settingsService;

    /// <summary>Get company settings (name + logo URL). Available publicly so the login page can display company name.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CompanySettingsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var settings = await _settingsService.GetAsync(ct);
        return Ok(ApiResponse<CompanySettingsDto>.Ok(settings));
    }

    /// <summary>Update company name. Admin only.</summary>
    [HttpPut]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<CompanySettingsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdateCompanySettingsDto dto, CancellationToken ct)
    {
        var settings = await _settingsService.UpdateAsync(dto, CurrentUser, ct);
        return Ok(ApiResponse<CompanySettingsDto>.Ok(settings, "Settings updated."));
    }

    /// <summary>Upload company logo. Admin only. Accepts multipart/form-data with field name "logo".</summary>
    [HttpPost("logo")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<CompanySettingsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5 MB
    public async Task<IActionResult> UploadLogo(IFormFile logo, CancellationToken ct)
    {
        if (logo is null || logo.Length == 0)
            return BadRequest(ApiResponse<object>.Fail("No file was uploaded."));

        await using var stream = logo.OpenReadStream();
        var url = await _settingsService.UploadLogoAsync(stream, logo.FileName, CurrentUser, ct);

        var settings = await _settingsService.GetAsync(ct);

        return Ok(ApiResponse<CompanySettingsDto>.Ok(settings, "Logo uploaded."));
    }

    /// <summary>Update government mandate default rates. Admin only.</summary>
    [HttpPut("deductions")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<CompanySettingsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateDeductions([FromBody] UpdateDeductionSettingsDto dto, CancellationToken ct)
    {
        var settings = await _settingsService.UpdateDeductionSettingsAsync(dto, CurrentUser, ct);
        return Ok(ApiResponse<CompanySettingsDto>.Ok(settings, "Deduction settings updated."));
    }
}
