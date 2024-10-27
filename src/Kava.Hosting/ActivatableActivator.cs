using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Kava.Hosting.Abstractions;

namespace Kava.Hosting;

public static class ActivatableActivator
{
    public static void RegisterEvents(IActivable viewModel, Control control)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(control);

        control.Loaded += Loaded;
        control.Unloaded += Unloaded;
        return;

        void Loaded(object? sender, RoutedEventArgs e)
        {
            viewModel?.Activate();
        }

        void Unloaded(object? sender, RoutedEventArgs e)
        {
            viewModel?.Deactivate();

            control.Loaded -= Loaded;
            control.Unloaded -= Unloaded;
        }
    }
}
