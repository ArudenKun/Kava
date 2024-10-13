using System.ComponentModel;

namespace Kava.ViewModels.Abstractions;

public interface IViewModel
    : IDisposable,
        INotifyPropertyChanged,
        INotifyPropertyChanging,
        INotifyDataErrorInfo
{
    event EventHandler? Loaded;
    event EventHandler? Unloaded;
    event EventHandler? AttachedToVisualTree;
    event EventHandler? DetachedFromVisualTree;
    void OnLoaded();
    void OnUnloaded();
    void OnAttachedToVisualTree();
    void OnDetachedFromVisualTree();
}
