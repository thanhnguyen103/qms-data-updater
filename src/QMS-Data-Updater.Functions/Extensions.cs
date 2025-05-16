using Microsoft.Extensions.DependencyInjection;

namespace QMS_Data_Updater.Functions;

// register the handlers
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Application.AssemblyMarker>());
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserCertificateService, UserCertificateService>();
        return services;
    }
}