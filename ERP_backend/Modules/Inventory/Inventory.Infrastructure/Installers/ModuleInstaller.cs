using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PineConePro.Erp.Inventory.Infrastructure.Extensions;

namespace PineConePro.Erp.Inventory.Infrastructure.Installers;

/// <summary>
/// Default installer that wires the module's infrastructure into the host.
/// </summary>
public sealed class ModuleInstaller : IModuleInstaller
{
    /// <inheritdoc />
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
    }
}