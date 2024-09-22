using System;
using System.ComponentModel;

namespace Desktop.ViewModels.Abstractions;

public interface IViewModel
    : IDisposable,
        INotifyPropertyChanged,
        INotifyPropertyChanging,
        INotifyDataErrorInfo
{
    void Loaded();
    void Unloaded();
    void AttachedToVisualTree();
    void DetachedFromVisualTree();
}
