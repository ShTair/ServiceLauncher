namespace ServiceLauncher.Services;

internal class Watcher : IDisposable
{
    private readonly string _target;
    private readonly FileSystemWatcher _watcher;

    public event Action<string>? Changed;

    public Watcher(string target)
    {
        _target = target;

        _watcher = new FileSystemWatcher(_target);

        _watcher.Changed += OnChanged;
        _watcher.Deleted += OnChanged;
        _watcher.Created += OnChanged;
        _watcher.Renamed += OnChanged;

        _watcher.IncludeSubdirectories = true;
        _watcher.EnableRaisingEvents = true;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.Name is null) return;
        Changed?.Invoke(e.Name);
    }

    public void Dispose()
    {
        _watcher.Dispose();
    }
}
