using System;
using Avalonia.Controls;
using Kava.Generators.Attributes;
using Kava.Hosting;
using Kava.ViewModels.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Kava;

[StaticViewLocator]
public sealed partial class ViewLocator
{
    private readonly IServiceProvider _serviceProvider;

    public ViewLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Control? Build(object? viewModel)
    {
        if (viewModel is null)
            return null;

        var viewModelType = viewModel.GetType();

        // if (!ViewMap.TryGetValue(viewModelType, out var factory))
        //     return new TextBlock { Text = $"No view registered for {viewModelType.FullName}" };
        if (!ViewMapTypes.TryGetValue(viewModelType, out var viewType))
        {
            return new TextBlock { Text = $"No view registered for {viewModelType.FullName}" };
        }

        var control = (Control)_serviceProvider.GetRequiredService(viewType);
        control.DataContext = viewModel;
        ActivatableActivator.RegisterEvents((IViewModel)viewModel, control);
        return control;
    }

    public bool Match(object? data) => data is IViewModel;
}
