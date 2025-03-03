using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VasttrafikStopInformation.Infrastructure.Extensions;

// Create a host builder
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        var env = hostContext.HostingEnvironment;
        config.SetBasePath(AppContext.BaseDirectory); // Set the base directory to the execution path
        config.AddJsonFile("appsettings.json", optional: false);
        config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
        config.AddEnvironmentVariables();
        config.AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {

        // Add infrastructure services
        services.AddInfrastructureServices(hostContext.Configuration);

        // Register formatter
        services.AddSingleton<VasttrafikStopInformation.Core.Interfaces.IDepartureFormatter,
            VasttrafikStopInformation.Console.Services.ConsoleDepartureFormatter>();

        // Register hosted service
        services.AddHostedService<VasttrafikStopInformation.Console.Services.DepartureConsoleService>();
    })
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

// Run the application
await host.RunAsync();
