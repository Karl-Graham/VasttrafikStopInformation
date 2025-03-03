using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VasttrafikStopInformation.Core.Interfaces;
using VasttrafikStopInformation.Core.Settings;
using VasttrafikStopInformation.Infrastructure.Services;
using VasttrafikStopInformation.Infrastructure.Settings;

namespace VasttrafikStopInformation.Infrastructure.Extensions;


public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.Configure<VasttrafikApiSettings>(
            configuration.GetSection("VasttrafikApi"));

        services.AddSingleton<IValidateOptions<VasttrafikApiSettings>, VasttrafikApiSettingsValidator>();

        services.AddSingleton<ITokenService, VasttrafikTokenService>();

        services.AddHttpClient<IDepartureService, VasttrafikDepartureService>();

        return services;
    }
}