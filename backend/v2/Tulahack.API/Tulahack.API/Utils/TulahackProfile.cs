using AutoMapper;
using Tulahack.API.Dto;
using Tulahack.API.Models;

namespace Tulahack.API.Utils;

public class TulahackProfile : Profile
{
    public TulahackProfile()
    {
        CreateMap<Account, Manager>(MemberList.None);
        CreateMap<Account, Superuser>(MemberList.None);

        CreateMap<Account, PersonBaseDto>();
        CreateMap<Manager, ManagerDto>();
    }
}