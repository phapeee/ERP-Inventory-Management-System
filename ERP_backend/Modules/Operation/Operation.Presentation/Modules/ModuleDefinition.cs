using Microsoft.AspNetCore.Routing;
using PineConePro.Erp.Operation.Presentation.Extensions;

namespace PineConePro.Erp.Operation.Presentation.Modules;

/// <summary>
/// Default module definition that wires controller-based endpoints.
/// </summary>
public sealed class ModuleDefinition : IModule
{
    /// <inheritdoc />
    public string Name => "Operation";

    /// <inheritdoc />
    public string HealthEndpointUrl => "/api/my-aggregates/health";

    /// <inheritdoc />
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapOperationEndpoints();
    }
}
