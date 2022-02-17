using ShComp.Threading;

namespace ServiceLauncher.Services;

internal class Updater : IAsyncDisposable
{
    private readonly Spacer _spacer = new(TimeSpan.FromSeconds(10));

    private readonly ServiceInfo _info;
    private readonly Service _service;

    private readonly Watcher _watcher;

    public Updater(ServiceInfo info, ServiceFactory serviceFactory)
    {
        _info = info;
        _service = serviceFactory.Create(info);

        _watcher = new Watcher(info.SourcePath);
    }

    public async ValueTask DisposeAsync()
    {
        await _service.DisposeAsync();
    }

    public async Task RestartAsync()
    {
        await _spacer.InvokeAsync(async () =>
        {
            await _service.StopAsync();
            await UpdateAsync();
            _service.Start();
        });
    }

    private async Task UpdateAsync()
    { }
}

internal class UpdaterFactory
{
    private readonly ServiceFactory _serviceFactory;

    public UpdaterFactory(ServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    public Updater Create(ServiceInfo info)
    {
        return new Updater(info, _serviceFactory);
    }
}
