using Microsoft.Extensions.Configuration;
using ServiceLauncher.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ServiceLauncher.Test;

public class WatcherTest
{
    private readonly IConfiguration _configuration;

    public WatcherTest()
    {
        _configuration = new ConfigurationBuilder().AddUserSecrets<WatcherTest>().Build();
    }

    [Fact]
    public async Task ChangedTest()
    {
        var target = _configuration["Watcher:Target"];
        if (!Directory.Exists(target)) Directory.CreateDirectory(target);

        var tcs = new TaskCompletionSource<string>();
        void Handler(string name)
        {
            tcs.TrySetResult(name);
        }

        using var watcher = new Watcher(target);
        watcher.Changed += Handler;
        try
        {
            await File.WriteAllTextAsync(Path.Combine(target, "dummy.txt"), "dummy");
            var name = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(10));
            Assert.Equal("dummy.txt", name);
        }
        finally
        {
            watcher.Changed -= Handler;
            Directory.Delete(target, true);
        }
    }
}
