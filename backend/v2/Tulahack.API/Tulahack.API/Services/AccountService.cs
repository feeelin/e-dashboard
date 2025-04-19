using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tulahack.API.Context;
using Tulahack.API.Dto;
using Tulahack.API.Extensions;
using Tulahack.API.Models;
using Tulahack.API.Utils;

namespace Tulahack.API.Services;

public interface IAccountService
{
    Task<Account?> GetAccount(Guid userId);
    Task<ManagerDto?> GetManagerDetails(Guid userId);

    Task<PersonBaseDto?> UpdateAccount(PersonBaseDto dto);

    Task<Account> CreateAccount(string jwt);
    Task<PersonBaseDto?> CreateAccount(PersonBaseDto dto);

    Task<Account?> DeleteAccount(Guid getUserId);
    Task<Account> RefreshAccess(string jwt);
    Task<ManagerDto?> UpdateManager(ManagerDto dto);
}

public class AccountService : IAccountService
{
    private readonly IMapper _mapper;
    private readonly ITulahackContext _tulahackContext;
    private readonly CdnConfiguration _cdnConfiguration;

    public AccountService(
        ITulahackContext tulahackContext,
        IOptions<CdnConfiguration> cdnConfiguration,
        IMapper mapper)
    {
        _tulahackContext = tulahackContext;
        _cdnConfiguration = cdnConfiguration.Value;
        _mapper = mapper;
    }

    public Task<PersonBaseDto?> CreateAccount(PersonBaseDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Account> RefreshAccess(string jwt)
    {
        JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
        Account user = _tulahackContext.Accounts
            .AsTracking()
            .First(item => item.Id == Guid.Parse(token.Subject));

        user.Role = jwt.GetRole();
        await _tulahackContext.SaveChangesAsync();
        return user;
    }

    public Task<ManagerDto?> UpdateManager(ManagerDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Account> CreateAccount(string jwt)
    {
        JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

        var user = new Account
        {
            Id = Guid.Parse(token.Subject),
            Firstname = token.Claims.First(item => item.Type == "given_name").Value,
            Lastname = token.Claims.First(item => item.Type == "family_name").Value,
            Email = token.Claims.First(item => item.Type == "email").Value,
            PhotoUrl = string.Concat(_cdnConfiguration.CdnUrl, "avatar/hacker.png"),
            Role = TulahackRole.Visitor,
        };
        _ = _tulahackContext.Accounts.Add(user);
        await _tulahackContext.SaveChangesAsync();
        return user;
    }

    public async Task<ManagerDto?> GetManagerDetails(Guid userId)
    {
        Manager? account = await _tulahackContext
            .Managers
            .FirstOrDefaultAsync(user => user.Id == userId);

        if (account is null)
        {
            return null;
        }

        List<Manager> team = await _tulahackContext
            .Managers
            .ToListAsync();

        ManagerDto result = _mapper.Map<ManagerDto>(account);

        return result;
    }
    
    public async Task<Account?> GetAccount(Guid userId)
    {
        Account? account = await _tulahackContext
            .Accounts
            .FirstOrDefaultAsync(user => user.Id == userId);

        return account;
    }

    public Task<PersonBaseDto?> UpdateAccount(PersonBaseDto dto)
    {
        throw new NotImplementedException();
    }


    public Task<Account?> DeleteAccount(Guid getUserId)
    {
        throw new NotImplementedException();
    }
}