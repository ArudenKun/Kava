using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Desktop.ViewModels.Abstractions;
using ZiggyCreatures.Caching.Fusion;

namespace Desktop.ViewModels;

public sealed partial class MainWindowViewModel : BaseViewModel
{
    private readonly IFusionCache _fusionCache;

    public MainWindowViewModel(IFusionCache fusionCache)
    {
        _fusionCache = fusionCache;
    }

#pragma warning disable CA1822 // Mark members as static
    public string Greeting => "Welcome to Avalonia!";
#pragma warning restore CA1822 // Mark members as static

    [RelayCommand]
    private async Task Test()
    {
        _ = await _fusionCache.GetOrSetAsync(
            $"{Random.Shared.Next(0, 100)}-key",
            $"TestValue-{Random.Shared.Next(0, 100)}"
        );
    }
}
