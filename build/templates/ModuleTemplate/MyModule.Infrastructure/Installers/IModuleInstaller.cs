using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PineConePro.Erp.MyModule.Infrastructure.Installers;

/// <summary>
/// Provides a contract for registering module dependencies with the host.
/// </summary>
public interface IModuleInstaller
{
    /// <summary>
    /// Registers module services with the supplied service collection.
    /// </summary>
    /// <param name="services">The target service collection.</param>
    /// <param name="configuration">The host configuration.</param>
    void Install(IServiceCollection services, IConfiguration configuration);
}
