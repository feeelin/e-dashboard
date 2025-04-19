using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tulahack.API.Dto;
using Tulahack.API.Extensions;
using Tulahack.API.Models;
using Tulahack.API.Services;
using Tulahack.API.Utils;

namespace Tulahack.API.Controllers;

[ApiController]
[UserIdActionFilter]
[Authorize(Policy = "Public+")]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PersonBaseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        Account? user = await _accountService.GetAccount(HttpContext.User.GetUserId());
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("Manager")]
    [ProducesResponseType(typeof(ManagerDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManager()
    {
        ManagerDto? user = await _accountService.GetManagerDetails(HttpContext.User.GetUserId());
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPatch]
    [ProducesResponseType(typeof(PersonBaseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Patch([FromBody] PersonBaseDto dto)
    {
        PersonBaseDto? user = await _accountService.UpdateAccount(dto);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPatch("Manager")]
    [ProducesResponseType(typeof(ManagerDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> PatchManager([FromBody] ManagerDto dto)
    {
        ManagerDto? user = await _accountService.UpdateManager(dto);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    
    [HttpPost]
    [ProducesResponseType(typeof(PersonBaseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] PersonBaseDto dto)
    {
        PersonBaseDto? user = await _accountService.CreateAccount(dto);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }


    [HttpDelete]
    [ProducesResponseType(typeof(PersonBaseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete()
    {
        Account? user = await _accountService.DeleteAccount(HttpContext.User.GetUserId());
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}