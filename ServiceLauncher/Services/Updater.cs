using ShComp.IO;
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
        _watcher.Dispose();
        await _service.DisposeAsync();
    }

    public async Task RestartAsync()
    {
        await _spacer.InvokeAsync(async () =>
        {
            await _service.StopAsync();
            await Task.Delay(_info.Delay);
            Update();
            _service.Start();
        });
    }

    public void Update()
    {
        if (_service.IsRunning) throw new InvalidOperationException();

        var target = Path.GetFullPath(_info.SourcePath);
        foreach (var pathInfo in FileUtils.GetPathInfos(target))
        {
            var newPath = Path.Combine(_info.WorkingPath, pathInfo.SubPath);
            if (pathInfo.IsDirectory)
            {
                if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);
            }
            else
            {
                File.Copy(pathInfo.Path, newPath, true);
            }
        }
    }
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
