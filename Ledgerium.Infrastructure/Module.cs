using Ledgerium.Application.Abstractions;
using Ledgerium.Application.Repositories;
using Ledgerium.Application.Services;
using Ledgerium.Infrastructure.Mapping;
using Ledgerium.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ledgerium.Infrastructure;

public sealed class InfrastructureModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IBalanceRepository, BalanceRepository>();
        services.AddScoped<IDbTransactionManager, DbTransactionManager>();
        
        services.AddAutoMapper(cfg => {
            cfg.AddProfile<BalanceEntitiesProfile>();
            cfg.AddProfile<TransactionProfile>();
        }, AppDomain.CurrentDomain.GetAssemblies());
    }
}