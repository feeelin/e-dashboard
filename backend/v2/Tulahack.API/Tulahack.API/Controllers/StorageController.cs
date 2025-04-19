using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tulahack.API.Extensions;
using Tulahack.API.Models;
using Tulahack.API.Services;
using Tulahack.API.Utils;

namespace Tulahack.API.Controllers;

[ApiController]
[UserIdActionFilter]
[Authorize(Policy = "Public+")]
[Route("api/[controller]")]
public class StorageController : ControllerBase
{
    private readonly IStorageService _storageService;

    public StorageController(
        IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(StorageFile), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid fileId)
    {
        StorageFile? result = await _storageService.GetFile(HttpContext.User.GetUserId(), fileId, default);

        if (result is null)
        {
            return NotFound("Cannot find file for provided FileId");
        }

        return Ok(result);
    }

    [HttpGet("file")]
    public async Task<IActionResult> GetFile(Guid fileId)
    {
        StorageFile? result = await _storageService.GetFile(HttpContext.User.GetUserId(), fileId, default);

        if (result is null)
        {
            return NotFound("Cannot find file for provided FileId");
        }

        await using var stream = new FileStream(result.Filepath, FileMode.Open, FileAccess.Read);
        return File(stream, MediaTypeNames.Application.Octet, result.Filename);
    }

    [HttpGet("{teamId:guid}/files")]
    [ProducesResponseType(typeof(IEnumerable<StorageFile>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFiles(Guid teamId)
    {
        List<StorageFile> result = await _storageService.GetTeamFiles(teamId);
        return Ok(result);
    }

    [HttpGet("files")]
    [ProducesResponseType(typeof(IEnumerable<StorageFile>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFiles()
    {
        List<StorageFile> result = await _storageService.GetFiles(HttpContext.User.GetUserId());
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(StorageFile), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(IFormFile files, [FromQuery] FilePurposeType purposeType)
    {
        StorageFile result = await _storageService.UploadFile(
            files,
            purpose: purposeType,
            userId: HttpContext.User.GetUserId(),
            default);
        return Ok(result);
    }

    [HttpPatch]
    [ProducesResponseType(typeof(StorageFile), StatusCodes.Status200OK)]
    public Task<IActionResult> Patch() =>
        Task.FromResult<IActionResult>(Ok());

    [HttpDelete]
    [ProducesResponseType(typeof(StorageFile), StatusCodes.Status200OK)]
    public Task<IActionResult> Delete() => Task.FromResult<IActionResult>(Ok());
}