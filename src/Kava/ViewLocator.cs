using Avalonia.Controls;
using Avalonia.Interactivity;
using Kava.Generators.Attributes;
using Kava.ViewModels.Abstractions;

namespace Kava;

[StaticViewLocator]
public sealed partial class ViewLocator
{
    public Control? Build(object? viewModel)
    {
        if (viewModel is null)
            return null;

        var viewModelType = viewModel.GetType();

        if (!ViewMap.TryGetValue(viewModelType, out var factory))
            return new TextBlock { Text = $"No view registered for {viewModelType.FullName}" };

        var control = factory(viewModel);
        control.DataContext = viewModel;
        RegisterEvents((IViewModel)viewModel, control);
        return control;
    }

    public bool Match(object? data) => data is IViewModel;

    private static void RegisterEvents(IViewModel viewModel, Control control)
    {
        control = control ?? throw new ArgumentNullException(nameof(control));

        control.Loaded += Loaded;
        control.Unloaded += Unloaded;
        return;

        void Loaded(object? sender, RoutedEventArgs e)
        {
            viewModel?.OnLoaded();
        }

        void Unloaded(object? sender, RoutedEventArgs e)
        {
            viewModel?.OnUnloaded();

            control.Loaded -= Loaded;
            control.Unloaded -= Unloaded;
        }
    }
}
