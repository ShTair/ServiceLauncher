global using Microsoft.Extensions.Options;
global using ServiceLauncher.Models;
global using ServiceLauncher.Services;
using ShComp.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ServiceLauncher.Test")]

TimeTextWriter.SetOut();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddSingleton<UpdaterFactory>();
        services.AddSingleton<ServiceFactory>();

        services.Configure<List<ServiceInfo>>(configuration.GetSection("Services"));
        services.AddSingleton<Launcher>();
    })
    .Build();

var launcher = host.Services.GetRequiredService<Launcher>();

await host.RunAsync();
