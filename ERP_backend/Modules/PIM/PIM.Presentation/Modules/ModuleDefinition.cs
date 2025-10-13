using Microsoft.AspNetCore.Routing;
using PineConePro.Erp.PIM.Presentation.Extensions;

namespace PineConePro.Erp.PIM.Presentation.Modules;

/// <summary>
/// Default module definition that wires controller-based endpoints.
/// </summary>
public sealed class ModuleDefinition : IModule
{
    /// <inheritdoc />
    public string Name => "PIM";

    /// <inheritdoc />
    public string HealthEndpointUrl => "/api/my-aggregates/health";

    /// <inheritdoc />
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPIMEndpoints();
    }
}
