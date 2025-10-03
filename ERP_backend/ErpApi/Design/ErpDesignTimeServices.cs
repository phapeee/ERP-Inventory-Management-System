using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;

namespace ErpApi.Design;

[SuppressMessage("Design", "CA1515:Consider marking type as internal", Justification = "EF Core design-time discovery requires a public implementation of IDesignTimeServices.")]
public sealed class ErpDesignTimeServices : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMigrationsCodeGenerator, SuppressingCSharpMigrationsGenerator>();
    }
}
