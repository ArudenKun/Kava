using System;
using System.Text.Json.Serialization;
using Avalonia.Styling;
using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Helpers;
using Desktop.Models;
using Desktop.Services.Abstractions;
using LiteDB;
using Material.Icons;
using R3;

namespace Desktop.Services;

[INotifyPropertyChanged]
public sealed partial class SettingsService : SettingsBase, IDisposable, ISingleton
{
    private readonly IDisposable _saveIntervalSubscription;
    private readonly ILiteDatabase _liteDatabase;

    public SettingsService(ILiteDatabase liteDatabase)
        : base(EnvironmentHelper.AppDataDirectory.JoinPath("settings.json"), JsonContext.Default)
    {
        _liteDatabase = liteDatabase;
        _saveIntervalSubscription = Observable
            .Interval(TimeSpan.FromMinutes(5))
            .Skip(1)
            .Subscribe(_ => Save());
    }

    private bool _isAutoUpdateEnabled;

    public bool IsAutoUpdateEnabled
    {
        get => _isAutoUpdateEnabled;
        set => SetProperty(ref _isAutoUpdateEnabled, value);
    }

    private Theme _theme;

    public Theme Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    [JsonIgnore]
    public ThemeVariant ThemeVariant =>
        Theme switch
        {
            Theme.Light => ThemeVariant.Light,
            Theme.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default,
        };

    [JsonIgnore]
    public MaterialIconKind ThemeIconKind =>
        Theme switch
        {
            Theme.Light => MaterialIconKind.WhiteBalanceSunny,
            Theme.Dark => MaterialIconKind.MoonWaningCrescent,
            _ => MaterialIconKind.ThemeLightDark,
        };

    public override void Save()
    {
        base.Save();
        _liteDatabase.Checkpoint();
    }

    public void Dispose()
    {
        _saveIntervalSubscription.Dispose();
        Save();
    }

    [JsonSerializable(typeof(SettingsService))]
    [JsonSourceGenerationOptions(UseStringEnumConverter = true)]
    private sealed partial class JsonContext : JsonSerializerContext;
}
