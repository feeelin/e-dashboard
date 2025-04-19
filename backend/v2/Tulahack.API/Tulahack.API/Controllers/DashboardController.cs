using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tulahack.API.Dto;
using Tulahack.API.Services;

namespace Tulahack.API.Controllers;

[ApiController]
[Authorize(Policy = "Public+")]
[Route("api/[controller]")]
[ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(
        IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Get()
    {
        DashboardDto dashboard = await _dashboardService.GetOverview();
        return Ok(dashboard);
    }
}