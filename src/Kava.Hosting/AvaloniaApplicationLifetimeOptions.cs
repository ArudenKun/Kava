using Avalonia.Controls;

namespace Kava.Hosting;

public class AvaloniaApplicationLifetimeOptions
{
    public string[] Args { get; set; } = [];
    public ShutdownMode ShutdownMode { get; set; } = ShutdownMode.OnLastWindowClose;
}
