using Microsoft.Extensions.Configuration;
using ServiceLauncher.Models;
using ServiceLauncher.Services;
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

    [Fact]
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
}
