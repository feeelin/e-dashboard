using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tulahack.API.Dto;
using Tulahack.API.Utils;

namespace Tulahack.API.Controllers;

[ApiController]
[UserIdActionFilter]
[Authorize(Policy = "Public+")]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PersonBaseDto), StatusCodes.Status200OK)]
    public Task<IActionResult> Get() =>
        Task.FromResult<IActionResult>(Ok());

    [HttpPost]
    [ProducesResponseType(typeof(PersonBaseDto), StatusCodes.Status200OK)]
    public Task<IActionResult> Create() =>
        Task.FromResult<IActionResult>(Ok());

    [HttpPatch]
    [ProducesResponseType(typeof(PersonBaseDto), StatusCodes.Status200OK)]
    public Task<IActionResult> Patch() =>
        Task.FromResult<IActionResult>(Ok());

    [HttpDelete]
    [ProducesResponseType(typeof(PersonBaseDto), StatusCodes.Status200OK)]
    public Task<IActionResult> Delete() =>
        Task.FromResult<IActionResult>(Ok());
}