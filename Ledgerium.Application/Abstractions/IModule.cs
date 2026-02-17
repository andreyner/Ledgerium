using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ledgerium.Application.Abstractions;

public interface IModule
{
    void Register(IServiceCollection services, IConfiguration configuration);
}