using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Tulahack.API.Context;
using Tulahack.API.Extensions;
using Tulahack.API.Models;

namespace Tulahack.API.Services;

public interface IStorageService
{
    Task<StorageFile> UploadFile(IFormFile formFile, Guid userId, CancellationToken cancellationToken);
    Task<StorageFile> UploadFile(IFormFile formFile, FilePurposeType purpose, Guid userId, CancellationToken cancellationToken);
    Task<StorageFile?> GetFile(Guid userId, Guid fileId, CancellationToken cancellationToken);
    Task<List<StorageFile>> GetFiles(Guid personId);
    Task<List<StorageFile>> GetFiles(Guid personId, FilePurposeType purpose);
    Task<List<StorageFile>> GetTeamFiles(Guid teamId);
    Task<List<StorageFile>> GetTeamFiles(Guid teamId, FilePurposeType purpose);
}

public class StorageService : IStorageService
{
    private readonly IAccountService _accountService;
    private readonly ITulahackContext _tulahackContext;
    private readonly ILogger<StorageService> _logger;

    public StorageService(
        IAccountService accountService,
        ITulahackContext tulahackContext,
        ILogger<StorageService> logger)
    {
        _accountService = accountService;
        _tulahackContext = tulahackContext;
        _logger = logger;
    }

    private async Task<Guid> GetStorageId(Account person)
    {
        switch (person.Role)
        {
            case TulahackRole.Manager:
                // Manager Manager = await _tulahackContext.Managers
                //     .Include(Manager => Manager.Team)
                //     .FirstAsync(user => user.Id == person.Id);
                // return Manager.Team?.Id ?? Guid.Empty;
            case TulahackRole.Superuser:
            case TulahackRole.Visitor:
            default:
                return person.Id;
        }
    }

    public async Task<StorageFile> UploadFile(
        IFormFile formFile,
        Guid userId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("got a new file");

        Account? userAccount = await _accountService.GetAccount(userId);
        if (userAccount is null)
        {
            throw new ValidationException("Unable to find user account in DB, re-login and try again");
        }

        var filePath = userId.GetStoragePath(userAccount.Role, formFile.FileName);
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ValidationException("Invalid file path");
        }

        _ = Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? "/appData/storage/temp");
        await using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            await formFile.CopyToAsync(stream, cancellationToken);
        }

        var storageFile = new StorageFile
        {
            Filename = formFile.FileName,
            Filepath = filePath,
            Owner = userAccount.Id,
            Purpose = FilePurposeType.Unknown,
            CreationDate = DateTime.Now,
            Revision = default
        };

        _ = _tulahackContext.StorageFiles.Add(storageFile);
        await _tulahackContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("File {FilesName} uploaded!", formFile.FileName);
        return storageFile;
    }

    public async Task<StorageFile> UploadFile(
        IFormFile formFile,
        FilePurposeType purpose,
        Guid userId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Got a new file");

        Account? userAccount = await _accountService.GetAccount(userId);
        if (userAccount is null)
            throw new ValidationException("Unable to find user account in DB, re-login and try again");

        Guid ownerId = await GetStorageId(userAccount);
        if (ownerId == Guid.Empty)
            throw new ValidationException("Unable to get storage id for provided account");

        var filePath = ownerId.GetStoragePath(userAccount.Role, purpose, formFile.FileName);
        if (string.IsNullOrEmpty(filePath))
            throw new ValidationException("Invalid file path");

        _ = Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? "/appData/storage/temp");
        await using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            await formFile.CopyToAsync(stream, cancellationToken);
        }

        var revision = _tulahackContext.StorageFiles
            .Where(item => item.Owner == ownerId)
            .Where(item => item.Purpose == purpose)
            .OrderBy(item => item.CreationDate)
            .Count();

        var storageFile = new StorageFile()
        {
            Filename = formFile.FileName,
            Filepath = filePath,
            Owner = userAccount.Id,
            Purpose = FilePurposeType.Unknown,
            CreationDate = DateTime.Now,
            Revision = revision
        };

        _ = _tulahackContext.StorageFiles.Add(storageFile);
        await _tulahackContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("File {FilesName} uploaded!", formFile.FileName);
        return storageFile;
    }

    public Task<StorageFile?> GetFile(Guid userId, Guid fileId, CancellationToken cancellationToken) =>
        _tulahackContext.StorageFiles
            .FirstOrDefaultAsync(item => item.Id == fileId, cancellationToken: cancellationToken);

    public Task<List<StorageFile>> GetFiles(Guid personId)
        => _tulahackContext.StorageFiles
            .Where(item => item.Owner == personId)
            .OrderBy(item => item.CreationDate)
            .ToListAsync();

    public Task<List<StorageFile>> GetFiles(Guid personId, FilePurposeType purpose)
        => _tulahackContext.StorageFiles
            .Where(item => item.Owner == personId)
            .Where(item => item.Purpose == purpose)
            .OrderBy(item => item.CreationDate)
            .ToListAsync();

    public Task<List<StorageFile>> GetTeamFiles(Guid teamId)
        => _tulahackContext.StorageFiles
            .Where(item => item.Owner == teamId)
            .OrderBy(item => item.CreationDate)
            .ToListAsync();

    public Task<List<StorageFile>> GetTeamFiles(Guid teamId, FilePurposeType purpose)
        => _tulahackContext.StorageFiles
            .Where(item => item.Owner == teamId)
            .Where(item => item.Purpose == purpose)
            .OrderBy(item => item.CreationDate)
            .ToListAsync();
}