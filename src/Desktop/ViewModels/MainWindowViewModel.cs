using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Extensions;
using Desktop.Services;
using Desktop.ViewModels.Abstractions;
using Material.Icons;
using SukiUI;
using ZiggyCreatures.Caching.Fusion;

namespace Desktop.ViewModels;

public sealed partial class MainWindowViewModel : BaseViewModel
{
    private readonly IFusionCache _fusionCache;

    [ObservableProperty]
    private MaterialIconKind _iconKind;

    public MainWindowViewModel(IFusionCache fusionCache, SettingsService settingsService)
    {
        _fusionCache = fusionCache;

        settingsService
            .WatchProperty(x => x.Theme, () => IconKind = settingsService.ThemeIconKind)
            .DisposeWith(Subscriptions);
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

    [RelayCommand]
    private void ChangeTheme() => SukiTheme.GetInstance().SwitchBaseTheme();
}
