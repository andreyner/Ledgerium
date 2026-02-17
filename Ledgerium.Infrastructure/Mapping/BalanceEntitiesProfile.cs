using AutoMapper;
using Ledgerium.Domain;
using Ledgerium.Infrastructure.Entities;

namespace Ledgerium.Infrastructure.Mapping;

public sealed class BalanceEntitiesProfile: Profile
{
    public BalanceEntitiesProfile()
    {
        CreateMap<BalanceEntity, Balance>(MemberList.Destination);
        CreateMap<Balance, BalanceEntity>(MemberList.Destination);
    }
}