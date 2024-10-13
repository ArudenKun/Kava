using System.ComponentModel;
using Avalonia;
using Avalonia.Interactivity;

namespace Kava.ViewModels.Abstractions;

public interface IViewModel
    : IDisposable,
        INotifyPropertyChanged,
        INotifyPropertyChanging,
        INotifyDataErrorInfo
{
    event Action<RoutedEventArgs>? Loaded;
    event Action<RoutedEventArgs>? Unloaded;
    event Action<VisualTreeAttachmentEventArgs>? AttachedToVisualTree;
    event Action<VisualTreeAttachmentEventArgs>? DetachedFromVisualTree;
    void OnLoaded(RoutedEventArgs e);
    void OnUnloaded(RoutedEventArgs e);
    void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e);
    void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e);
}
