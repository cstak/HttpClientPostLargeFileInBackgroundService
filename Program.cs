using System.Net;
using HttpClientPostLargeFileInBackgroundService;
using HttpClientPostLargeFileInBackgroundService.Interfaces;
using HttpClientPostLargeFileInBackgroundService.Services;

IHost host = Host.CreateDefaultBuilder(args)
/*
    //You can use backgroundservice as a windows service. It needs Microsoft.Extensions.Hosting namespace
    .UseWindowsService(options =>
    {
        options.ServiceName = "Large-File-Upload-Service";
    })
*/
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
         services.AddScoped<IFileConsumerService, FileConsumerService>();

        services.AddSingleton<IFileWatcherService, FileWatcherService>();
        services.AddHttpClient("Default", client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
        {
            DefaultProxyCredentials = CredentialCache.DefaultNetworkCredentials
        });
    })
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
        {
            configBuilder
                .SetBasePath(Path.GetDirectoryName(System.AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .Build();
        })
    .Build();

await host.RunAsync();
