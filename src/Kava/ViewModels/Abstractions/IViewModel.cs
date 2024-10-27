using System;
using System.ComponentModel;
using Kava.Hosting.Abstractions;

namespace Kava.ViewModels.Abstractions;

public interface IViewModel
    : IDisposable,
        INotifyPropertyChanged,
        INotifyPropertyChanging,
        INotifyDataErrorInfo,
        IActivable
{
    event Action Loaded;
    event Action Unloaded;
}

public interface ISingletonViewModel : IViewModel;

public interface ITransientViewModel : IViewModel;
