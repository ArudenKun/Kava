using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Extensions;
using Desktop.Services;
using Desktop.ViewModels.Abstractions;
using Material.Icons;
using Microsoft.Extensions.Logging;
using R3;
using SukiUI;
using ZiggyCreatures.Caching.Fusion;
using ZLogger;

namespace Desktop.ViewModels;

public sealed partial class MainWindowViewModel : BaseViewModel
{
    private readonly IFusionCache _fusionCache;
    private readonly ILogger<MainWindowViewModel> _logger;

    [ObservableProperty]
    private MaterialIconKind _iconKind;

    [ObservableProperty]
    private int _width;

    [ObservableProperty]
    private int _height;

    public MainWindowViewModel(
        IFusionCache fusionCache,
        SettingsService settingsService,
        ILogger<MainWindowViewModel> logger
    )
    {
        _fusionCache = fusionCache;
        _logger = logger;

        IconKind = settingsService.ThemeIconKind;

        settingsService
            .ObservePropertyChanged(x => x.Theme, false)
            .Subscribe(_ => IconKind = settingsService.ThemeIconKind)
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
    private static void ChangeTheme() => SukiTheme.GetInstance().SwitchBaseTheme();

    partial void OnHeightChanged(int value)
    {
        _logger.ZLogInformation($"Height: {value}");
    }

    partial void OnWidthChanged(int value)
    {
        _logger.ZLogInformation($"Width: {value}");
    }
}
