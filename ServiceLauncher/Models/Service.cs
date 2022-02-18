using System.Diagnostics;

namespace ServiceLauncher.Models;

internal class Service : IAsyncDisposable
{
    private Process? _process;

    public ServiceInfo Info { get; }

    public bool IsRunning => _process?.HasExited == false;

    public Service(ServiceInfo info)
    {
        Info = info;
    }

    public async Task StopAsync()
    {
        if (!IsRunning) return;
        if (_process is not { } process) return;

        var tcs = new TaskCompletionSource();
        void OnExited(object? sender, EventArgs e) { tcs.TrySetResult(); }

        process.Exited += OnExited;
        process.EnableRaisingEvents = true;

        if (!process.HasExited)
        {
            try
            {
                process.Kill();
                var cts = new CancellationTokenSource(10000);
                await tcs.Task.WaitAsync(cts.Token);
            }
            catch { }
        }

        process.Exited -= OnExited;
        process.Dispose();
    }

    public void Start()
    {
        if (IsRunning) throw new InvalidOperationException();

        var path = Path.Combine(Info.WorkingPath, Info.ExecutableFileName);
        var pi = new ProcessStartInfo(path) { };
        if (Process.Start(pi) is { } process)
        {
            _process = process;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }
}

internal class ServiceFactory
{
    public ServiceFactory()
    {
    }

    public Service Create(ServiceInfo info)
    {
        return new Service(info);
    }
}
