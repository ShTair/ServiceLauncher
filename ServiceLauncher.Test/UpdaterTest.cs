using Microsoft.Extensions.Configuration;
using ServiceLauncher.Models;
using ServiceLauncher.Services;
using ShComp;
using System.Threading.Tasks;
using Xunit;

namespace ServiceLauncher.Test;

public class UpdaterTest
{
    private readonly IConfiguration _configuration;

    public UpdaterTest()
    {
        _configuration = new ConfigurationBuilder().AddUserSecrets<WatcherTest>().Build();
    }

    [Fact(Skip = "old")]
    public async Task ChangedTest()
    {
        var info = new ServiceInfo
        {
            SourcePath = _configuration["Updater:SourcePath"],
            WorkingPath = _configuration["Updater:WorkingPath"],
        };

        await using var updater = new Updater(info, new ServiceFactory());
        updater.Update();
    }

    [Fact]
    public async Task RunAndRestart()
    {
        var info = new ServiceInfo
        {
            SourcePath = _configuration["Updater:SourcePath"],
            WorkingPath = _configuration["Updater:WorkingPath"],
            ExecutableFileSubPath = _configuration["Updater:ExecutableFileSubPath"],
            Delay = _configuration["Updater:Delay"].AsInt()!.Value,
        };

        await using var updater = new Updater(info, new ServiceFactory());

        await updater.RestartAsync();

        await Task.Delay(5000);

        await updater.RestartAsync();

        await Task.Delay(5000);
    }
}
