using Microsoft.Extensions.DependencyInjection;
using MyModule.Domain.Interfaces;
using MyModule.Infrastructure.Persistence.Repositories;

namespace MyModule.Infrastructure.Extensions;

/// <summary>
/// Contains extension methods for setting up infrastructure services.
/// </summary>
public static class ModuleExtensions
{
    /// <summary>
    /// Adds infrastructure services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IMyAggregateRepository, MyAggregateRepository>();

        // Register other services
        // services.AddScoped<IEmailService, MyEmailService>();

        // Add DbContext
        /*
        services.AddDbContext<Persistence.AppDbContext>(options =>
            options.UseSqlServer("your-connection-string")); // Or another provider
        */

        return services;
    }
}
