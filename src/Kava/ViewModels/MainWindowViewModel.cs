using Kava.ViewModels.Abstractions;

namespace Kava.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
#pragma warning disable CA1822 // Mark members as static
    public string Greeting => "Welcome to Avalonia!";
#pragma warning restore CA1822 // Mark members as static
}
