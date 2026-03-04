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
}
