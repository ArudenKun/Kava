using System.Timers;
using Avalonia.Interactivity;
using Avalonia.Reactive;
using SukiUI.Controls;

namespace Kava.Views;

public partial class MainWindow : SukiWindow
{
    private readonly Timer _idleTimer;
    private readonly TimeSpan _idleTimeout = TimeSpan.FromSeconds(5); // Set your idle timeout here
    private DateTime _lastInputTime;

    public MainWindow()
    {
        InitializeComponent();

        _idleTimer = new Timer(1000); // Check every second
        _idleTimer.Elapsed += CheckIdleStatus;
        _idleTimer.Start();

        KeyDownEvent.Raised.Subscribe(
            new AnonymousObserver<(object, RoutedEventArgs)>(tuple => OnUserInput(tuple.Item2))
        );
        PointerMovedEvent.Raised.Subscribe(
            new AnonymousObserver<(object, RoutedEventArgs)>(tuple => OnUserInput(tuple.Item2))
        );
    }

    private void CheckIdleStatus(object? sender, ElapsedEventArgs e)
    {
        Console.WriteLine("CheckIdleStatus");
        var timeSinceLastInput = DateTime.Now - _lastInputTime;
        ViewModel.IsIdle = timeSinceLastInput >= _idleTimeout;
    }

    private void OnUserInput(RoutedEventArgs e)
    {
        Console.WriteLine("OnUserInput");
        _lastInputTime = DateTime.Now; // Reset the timer on any input
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _idleTimer.Dispose(); // Clean up the timer when the window is closed
    }
}
