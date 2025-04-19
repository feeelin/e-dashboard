using AutoMapper;
using Tulahack.API.Dto;
using Tulahack.API.Models;

namespace Tulahack.API.Utils;

public class TulahackProfile : Profile
{
    public TulahackProfile()
    {
        CreateMap<PersonBase, Manager>(MemberList.None);
        CreateMap<PersonBase, Superuser>(MemberList.None);

        CreateMap<PersonBase, PersonBaseDto>();
        CreateMap<Manager, ManagerDto>();
    }
}