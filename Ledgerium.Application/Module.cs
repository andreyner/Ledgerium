using Ledgerium.Application.Abstractions;
using Ledgerium.Application.Commands;
using Ledgerium.Application.Dto;
using Ledgerium.Application.Handlers;
using Ledgerium.Application.Queries;
using Ledgerium.Application.Services;
using Ledgerium.Application.Services.CreditService;
using Ledgerium.Application.Services.DebitService;
using Ledgerium.Application.Services.RevertService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ledgerium.Application;

public sealed class ApplicationModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICommandHandler<CreditCommand, CreditResult>, CreditCommandHandler>();
        services.AddScoped<ICommandHandler<DebitCommand, DebitResult>, DebitCommandHandler>();
        services.AddScoped<ICommandHandler<RevertCommand, RevertResult>, RevertCommandHandler>();
        services.AddScoped<IQueryHandler<GetBalanceQuery, BalanceDto>, BalanceQueryHandler>();
        services.AddScoped<ICreditService, CreditService>();
        services.AddScoped<IDebitService, DebitService>();
        services.AddScoped<IRevertService, RevertService>();
    }
}