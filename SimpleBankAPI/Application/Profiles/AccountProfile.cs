using AutoMapper;
using SimpleBankAPI.Application.DataTransformationObjects.Responses;

namespace SimpleBankAPI.Application.Profiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<Domain.Entities.Account, AccountDto>().ReverseMap();
        CreateMap<Domain.Entities.Account, AccountBalance>();
    }
}