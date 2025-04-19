using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tulahack.API.Extensions;
using Tulahack.API.Models;
using Tulahack.API.Services;
using Tulahack.API.Utils;

namespace Tulahack.API.Controllers;

[ApiController]
[Authorize(Policy = "Public+")]
[Route("api/callback")]
[ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status302Found)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class CallbackController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly WebConfiguration _webConfiguration;

    public CallbackController(
        IAccountService accountService,
        IOptions<WebConfiguration> webConfiguration)
    {
        _accountService = accountService;
        _webConfiguration = webConfiguration.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register()
    {
        Claim ssoUserClaim = HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier);
        _ = Guid.TryParse(ssoUserClaim.Value, out Guid userId);

        var jwt = await HttpContext.GetTokenAsync("access_token");

        if (string.IsNullOrEmpty(jwt))
        {
            return Forbid("Incoming JwT token is in invalid format");
        }

        Account user = await _accountService.GetAccount(userId) ?? await _accountService.CreateAccount(jwt);

        if (user.Role != jwt.GetRole())
        {
            _ = await _accountService.RefreshAccess(jwt);
        }

        return Redirect(string.Concat(_webConfiguration.WebAppBaseUrl, "index.html"));
    }
}
