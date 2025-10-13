using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using PineConePro.Erp.PIM.Presentation.Modules;

namespace ErpApi.Hosting;

internal static class ModuleRegistrationExtensions
{
    private const string InstallerInterfaceName = "IModuleInstaller";
    private const string ModuleInterfaceName = "IModule";
    private const string InstallMethodName = "Install";

    public static IServiceCollection InstallDiscoveredModules(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var logger = services.BuildServiceProvider().GetRequiredService<ILogger<IModuleRegistry>>();

        foreach (var installerType in GetCandidateTypes(InstallerInterfaceName))
        {
            InvokeInstall(installerType, services, configuration, logger);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapDiscoveredModules(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var logger = endpoints.ServiceProvider.GetRequiredService<ILogger<IModuleRegistry>>();
        var moduleRegistry = endpoints.ServiceProvider.GetRequiredService<IModuleRegistry>();

        foreach (var moduleType in GetCandidateTypes(ModuleInterfaceName))
        {
            logger.LogInformation("Mapping endpoints for module {ModuleType}", moduleType.FullName);
            var module = (IModule)Activator.CreateInstance(moduleType)!;
            moduleRegistry.AddModule(module);
            module.MapEndpoints(endpoints);
        }

        return endpoints;
    }

    private static void InvokeInstall(Type installerType, IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        logger.LogInformation("Installing module from installer {InstallerType}", installerType.FullName);
        var instance = Activator.CreateInstance(installerType)
                      ?? throw new InvalidOperationException($"Unable to instantiate installer '{installerType.FullName}'.");

        var installMethod = ResolveMethod(installerType, InstallMethodName, typeof(IServiceCollection), typeof(IConfiguration));
        installMethod.Invoke(instance, new object[] { services, configuration });
    }

    private static MethodInfo ResolveMethod(Type type, string methodName, params Type[] parameterTypes)
    {
        var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        if (method is null)
        {
            throw new InvalidOperationException($"Type '{type.FullName}' must expose a public '{methodName}' method.");
        }

        var parameters = method.GetParameters();
        if (parameters.Length != parameterTypes.Length)
        {
            throw new InvalidOperationException(
                $"Method '{methodName}' on type '{type.FullName}' must accept {parameterTypes.Length} parameter(s).");
        }

        for (var index = 0; index < parameters.Length; index++)
        {
            if (!parameters[index].ParameterType.IsAssignableFrom(parameterTypes[index]))
            {
                throw new InvalidOperationException(
                    $"Parameter at position {index} on method '{methodName}' of type '{type.FullName}' must be assignable from '{parameterTypes[index].FullName}'.");
            }
        }

        return method;
    }

    private static IEnumerable<Type> GetCandidateTypes(string interfaceName)
    {
        return LoadCandidateAssemblies()
            .SelectMany(GetLoadableTypes)
            .Where(type =>
                type is { IsAbstract: false, IsInterface: false, FullName: not null } &&
                type.GetInterfaces().Any(i => string.Equals(i.Name, interfaceName, StringComparison.Ordinal)))
            .DistinctBy(type => type.FullName!)
            .OrderBy(type => type.FullName, StringComparer.Ordinal);
    }

    private static IEnumerable<Assembly> LoadCandidateAssemblies()
    {
        var assemblies = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic))
        {
            var name = assembly.GetName().Name;
            if (name is null)
            {
                continue;
            }

            assemblies[name] = assembly;
        }

        var dependencyContext = DependencyContext.Default;
        if (dependencyContext is not null)
        {
            foreach (var library in dependencyContext.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type == "project"))
            {
                if (assemblies.ContainsKey(library.Name))
                {
                    continue;
                }

                try
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    var name = assembly.GetName().Name;
                    if (name is not null)
                    {
                        assemblies[name] = assembly;
                    }
                }
                catch (FileNotFoundException)
                {
                    // Skip libraries that are not available at runtime.
                }
            }
        }

        return assemblies.Values;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetExportedTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
    }
}

public interface IModuleRegistry
{
    IEnumerable<IModule> Modules { get; }
    void AddModule(IModule module);
}

public class ModuleRegistry : IModuleRegistry
{
    private readonly List<IModule> _modules = new();
    public IEnumerable<IModule> Modules => _modules;

    public void AddModule(IModule module)
    {
        _modules.Add(module);
    }
}
