using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PayrollApi.API.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    /// <summary>Returns the authenticated user's identity name from JWT claims.</summary>
    protected string CurrentUser =>
        User.Identity?.Name ?? User.FindFirst("sub")?.Value ?? "system";

    /// <summary>Returns the authenticated user's numeric ID from the JWT 'sub' claim.</summary>
    protected int CurrentUserId =>
        int.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : 0;

    /// <summary>Returns the authenticated user's legacy role string from JWT claims.</summary>
    protected string CurrentUserRole =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;
}
