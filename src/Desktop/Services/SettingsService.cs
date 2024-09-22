using System;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using Avalonia.Styling;
using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Helpers;
using Desktop.Models;
using Desktop.Services.Abstractions;
using LiteDB;

namespace Desktop.Services;

[INotifyPropertyChanged]
public partial class SettingsService : SettingsBase, IDisposable, ISingleton
{
    private readonly IDisposable _subscription;
    private readonly ILiteDatabase _liteDatabase;

    public SettingsService(ILiteDatabase liteDatabase)
        : base(EnvironmentHelper.AppDataDirectory.JoinPath("settings.json"), JsonContext.Default)
    {
        _liteDatabase = liteDatabase;
        _subscription = Observable.Interval(TimeSpan.FromMinutes(5)).Skip(1).Subscribe(_ => Save());
    }

    #region Backing Fields

    private bool _isAutoUpdateEnabled = true;
    private Theme _theme = Theme.System;

    #endregion

    #region Properties

    public bool IsAutoUpdateEnabled
    {
        get => _isAutoUpdateEnabled;
        set => SetProperty(ref _isAutoUpdateEnabled, value);
    }

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

    #endregion

    public override void Save()
    {
        base.Save();
        _liteDatabase.Checkpoint();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _subscription.Dispose();
        Save();
    }

    [JsonSerializable(typeof(SettingsService))]
    [JsonSourceGenerationOptions(UseStringEnumConverter = true)]
    private sealed partial class JsonContext : JsonSerializerContext;
}
