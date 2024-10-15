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
    private readonly WeakEventManager _loadedEventManager = new();
    private readonly WeakEventManager _unloadedEventManager = new();
    private readonly WeakEventManager _attachedToVisualTreeEventManager = new();
    private readonly WeakEventManager _detachedFromVisualTree = new();

    public event EventHandler Loaded
    {
        add => _loadedEventManager.AddEventHandler(value);
        remove => _loadedEventManager.RemoveEventHandler(value);
    }

    public event EventHandler Unloaded
    {
        add => _unloadedEventManager.AddEventHandler(value);
        remove => _unloadedEventManager.RemoveEventHandler(value);
    }

    public event EventHandler AttachedToVisualTree
    {
        add => _attachedToVisualTreeEventManager.AddEventHandler(value);
        remove => _attachedToVisualTreeEventManager.RemoveEventHandler(value);
    }

    public event EventHandler DetachedFromVisualTree
    {
        add => _detachedFromVisualTree.AddEventHandler(value);
        remove => _detachedFromVisualTree.RemoveEventHandler(value);
    }

    public virtual void OnLoaded() =>
        _loadedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(Loaded));

    public virtual void OnUnloaded() =>
        _unloadedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(Unloaded));

    public virtual void OnAttachedToVisualTree() =>
        _attachedToVisualTreeEventManager.RaiseEvent(
            this,
            EventArgs.Empty,
            nameof(AttachedToVisualTree)
        );

    public virtual void OnDetachedFromVisualTree() =>
        _detachedFromVisualTree.RaiseEvent(this, EventArgs.Empty, nameof(DetachedFromVisualTree));

    public ISukiDialogManager DialogManager { get; } = new SukiDialogManager();
    public ISukiToastManager ToastManager { get; } = new SukiToastManager();

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

    ~BaseViewModel() => Dispose(false);

    protected void OnAllPropertiesChanged() => OnPropertyChanged(string.Empty);

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
