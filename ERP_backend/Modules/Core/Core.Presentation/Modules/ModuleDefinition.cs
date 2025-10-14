using Microsoft.AspNetCore.Routing;
using PineConePro.Erp.Core.Presentation.Extensions;

namespace PineConePro.Erp.Core.Presentation.Modules;

/// <summary>
/// Default module definition that wires controller-based endpoints.
/// </summary>
public sealed class ModuleDefinition : IModule
{
    /// <inheritdoc />
    public string Name => "Core";

    /// <inheritdoc />
    public string HealthEndpointUrl => "/api/my-aggregates/health";

    /// <inheritdoc />
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCoreEndpoints();
    }
}
