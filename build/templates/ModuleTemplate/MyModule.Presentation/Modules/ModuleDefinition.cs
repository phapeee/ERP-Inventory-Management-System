using Microsoft.AspNetCore.Routing;
using PineConePro.Erp.MyModule.Presentation.Extensions;

namespace PineConePro.Erp.MyModule.Presentation.Modules;

/// <summary>
/// Default module definition that wires controller-based endpoints.
/// </summary>
public sealed class ModuleDefinition : IModule
{
    /// <inheritdoc />
    public string Name => "MyModule";

    /// <inheritdoc />
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapMyModuleEndpoints();
    }
}
