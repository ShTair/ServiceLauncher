namespace ServiceLauncher.Services;

internal class ServiceFactory
{
    private readonly IServiceProvider _provider;

    public ServiceFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Service Create(ServiceInfo info)
    {
        return new Service(info);
    }
}
