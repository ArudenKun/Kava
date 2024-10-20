using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;

namespace Kava.Utilities.Extensions.Avalonia;

public static class AvaloniaExtensions
{
    public static Window? TryGetMainWindow(this IApplicationLifetime lifetime) =>
        lifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime
            ? desktopLifetime.MainWindow
            : null;

    public static Window? TryGetMainWindow(this Application app) =>
        app.ApplicationLifetime?.TryGetMainWindow();

    public static Window GetMainWindow(this Application application) =>
        application.TryGetMainWindow()
        ?? throw new ApplicationException("Could not find the main window.");

    public static Window GetMainWindow(this IApplicationLifetime lifetime) =>
        lifetime.TryGetMainWindow()
        ?? throw new ApplicationException("Could not find the main window.");

    public static TopLevel? TryGetTopLevel(this IApplicationLifetime lifetime) =>
        lifetime.TryGetMainWindow()
        ?? (lifetime as ISingleViewApplicationLifetime)?.MainView?.GetVisualRoot() as TopLevel;

    public static TopLevel? TryGetTopLevel(this Application app) => app.TryGetMainWindow();

    public static TopLevel GetTopLevel(this IApplicationLifetime lifetime) =>
        lifetime.TryGetTopLevel()
        ?? throw new ApplicationException("Could not find the top-level visual element.");

    public static TopLevel GetTopLevel(this Application application) =>
        application.TryGetTopLevel()
        ?? throw new ApplicationException("Could not find the top-level visual element.");

    public static IClipboard? TryGetClipboard(this IApplicationLifetime lifetime) =>
        lifetime.TryGetTopLevel()?.Clipboard;

    public static IClipboard GetClipboard(this IApplicationLifetime lifetime) =>
        lifetime.TryGetClipboard()
        ?? throw new ApplicationException("Could not find TopLevel clipboard.");

    public static ILauncher? TryGetLauncher(this IApplicationLifetime lifetime) =>
        lifetime.TryGetTopLevel()?.Launcher;

    public static ILauncher? TryGetLauncher(this Application application) =>
        application.TryGetTopLevel()?.Launcher;

    public static ILauncher GetLauncher(this IApplicationLifetime lifetime) =>
        lifetime.TryGetLauncher()
        ?? throw new ApplicationException("Could not find TopLevel launcher.");

    public static ILauncher GetLauncher(this Application application) =>
        application.TryGetLauncher()
        ?? throw new ApplicationException("Could not find TopLevel launcher.");

    public static bool TryShutdown(this IApplicationLifetime lifetime, int exitCode = 0)
    {
        switch (lifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktopLifetime:
                return desktopLifetime.TryShutdown(exitCode);
            case IControlledApplicationLifetime controlledLifetime:
                controlledLifetime.Shutdown(exitCode);
                return true;
            default:
                return false;
        }
    }
}
