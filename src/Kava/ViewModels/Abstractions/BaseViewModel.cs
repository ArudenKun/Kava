using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Kava.Utilities;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace Kava.ViewModels.Abstractions;

[ObservableRecipient]
public abstract partial class BaseViewModel : ObservableValidator, IViewModel
{
    private readonly WeakEventManager _weakEventManager = new();

    public event Action Loaded
    {
        add => _weakEventManager.AddEventHandler(value);
        remove => _weakEventManager.RemoveEventHandler(value);
    }

    public event Action Unloaded
    {
        add => _weakEventManager.AddEventHandler(value);
        remove => _weakEventManager.RemoveEventHandler(value);
    }

    public virtual void OnLoaded() => _weakEventManager.RaiseEvent(nameof(Loaded));

    public virtual void OnUnloaded() => _weakEventManager.RaiseEvent(nameof(Unloaded));

    public static ISukiDialogManager DialogManager { get; } = new SukiDialogManager();
    public static ISukiToastManager ToastManager { get; } = new SukiToastManager();

    protected DisposableCollector Subscriptions { get; } = new();

    protected static void Dispatch(Action action) => Dispatcher.UIThread.Invoke(action);

    /// <summary>
    /// Dispatches the specified action on the UI thread.
    /// </summary>
    /// <param name="action">The action to be dispatched.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected static async Task DispatchAsync(Action action, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await Dispatcher.UIThread.InvokeAsync(action);
    }

    /// <summary>
    /// Dispatches the specified action on the UI thread.
    /// </summary>
    /// <param name="action">The action to be dispatched.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected static async Task<TResult> DispatchAsync<TResult>(
        Func<TResult> action,
        CancellationToken cancellationToken
    )
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return default!;
        }

        return await Dispatcher.UIThread.InvokeAsync(action);
    }

    protected void OnAllPropertiesChanged() => OnPropertyChanged(string.Empty);

    ~BaseViewModel() => Dispose(false);

    /// <summary>
    /// Be sure to call base.Dispose() and is outside the if condition
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Subscriptions.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
