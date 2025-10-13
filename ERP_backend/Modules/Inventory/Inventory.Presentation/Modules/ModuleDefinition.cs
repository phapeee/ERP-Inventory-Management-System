using Microsoft.AspNetCore.Routing;
using PineConePro.Erp.Inventory.Presentation.Extensions;

namespace PineConePro.Erp.Inventory.Presentation.Modules;

/// <summary>
/// Default module definition that wires controller-based endpoints.
/// </summary>
public sealed class ModuleDefinition : IModule
{
    /// <inheritdoc />
    public string Name => "Inventory";

    /// <inheritdoc />
    public string HealthEndpointUrl => "/api/my-aggregates/health";

    /// <inheritdoc />
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapInventoryEndpoints();
    }
}
