using System;
using System.ComponentModel;

namespace Kava.ViewModels.Abstractions;

public interface IViewModel
    : IDisposable,
        INotifyPropertyChanged,
        INotifyPropertyChanging,
        INotifyDataErrorInfo
{
    event Action Loaded;
    event Action Unloaded;
    void Activate();
    void Deactivate();
}
