using System;
using Avalonia.Controls;
using M31.FluentApi.Attributes;

namespace Kava.Hosting;

[FluentApi]
public class AvaloniaHostOptions
{
    [FluentMember(0, "Set{Name}")]
    public string[] Args { get; private set; } = [];

    [FluentMember(0, "Set{Name}")]
    public ShutdownMode ShutdownMode { get; set; } = ShutdownMode.OnLastWindowClose;

    [FluentPredicate(0, "{Name}", "DoNot{Name}")]
    public bool SetMainWindow { get; set; } = false;
    internal Type MainWindowType { get; set; } = null!;
}
