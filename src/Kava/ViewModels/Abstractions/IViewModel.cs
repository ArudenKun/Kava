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
    void OnLoaded();
    void OnUnloaded();
}
