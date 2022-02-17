namespace ServiceLauncher.Services;

internal class Launcher : IAsyncDisposable
{
    private readonly IList<Updater> _updaters;

    public Launcher(IOptions<List<ServiceInfo>> options, UpdaterFactory updaterFactory)
    {
        _updaters = options.Value.Select(updaterFactory.Create).ToArray();
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(_updaters.Select(async t => await t.DisposeAsync()));
    }
}
