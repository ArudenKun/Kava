using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using R3;

namespace Desktop.ViewModels.Abstractions;

[ObservableRecipient]
public abstract partial class BaseViewModel : ObservableValidator, IViewModel
{
    private readonly WeakEventManager _weakEventManager = new();

    public event EventHandler? Loaded
    {
        add => _weakEventManager.AddEventHandler(value);
        remove => _weakEventManager.RemoveEventHandler(value);
    }

    public event EventHandler? Unloaded
    {
        add => _weakEventManager.AddEventHandler(value);
        remove => _weakEventManager.RemoveEventHandler(value);
    }

    public event EventHandler? AttachedToVisualTree
    {
        add => _weakEventManager.AddEventHandler(value);
        remove => _weakEventManager.RemoveEventHandler(value);
    }

    public event EventHandler? DetachedFromVisualTree
    {
        add => _weakEventManager.AddEventHandler(value);
        remove => _weakEventManager.RemoveEventHandler(value);
    }

    public virtual void OnLoaded()
    {
        _weakEventManager.RaiseEvent(nameof(Loaded));
    }

    public virtual void OnUnloaded()
    {
        _weakEventManager.RaiseEvent(nameof(Unloaded));
    }

    public virtual void OnAttachedToVisualTree()
    {
        _weakEventManager.RaiseEvent(nameof(AttachedToVisualTree));
    }

    public virtual void OnDetachedFromVisualTree()
    {
        _weakEventManager.RaiseEvent(nameof(DetachedFromVisualTree));
    }

    protected CompositeDisposable Subscriptions { get; } = new();

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
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected static async Task<TResult> DispatchAsync<TResult>(Func<TResult> action) =>
        await Dispatcher.UIThread.InvokeAsync(action);

    ~BaseViewModel()
    {
        Dispose(false);
    }

    protected void OnAllPropertiesChanged()
    {
        OnPropertyChanged(string.Empty);
    }

    protected virtual void Dispose(bool disposing) { }

    public void Dispose()
    {
        Dispose(true);
        Subscriptions.Dispose();
        GC.SuppressFinalize(this);
    }
}
