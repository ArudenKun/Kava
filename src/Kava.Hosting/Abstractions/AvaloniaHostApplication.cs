using System;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace Kava.Hosting.Abstractions;

public abstract class AvaloniaHostApplication : Application
{
    internal IServiceProvider InternalServices { get; set; } = null!;

    public IServiceProvider Services => InternalServices;
    public abstract void ConfigureServices(IServiceCollection services);
}
