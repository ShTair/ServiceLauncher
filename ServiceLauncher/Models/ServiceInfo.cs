#pragma warning disable CS8618

namespace ServiceLauncher.Models;

internal class ServiceInfo
{
    public string Name { get; set; }

    public string SourcePath { get; set; }

    public string WorkingPath { get; set; }

    public string ExecutableFileName { get; set; }

    public int Delay { get; set; }
}
