using System.Text.Json.Serialization;
using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using Kava.Services.Abstractions;
using SukiUI.Enums;

namespace Kava.Services;

[INotifyPropertyChanged]
public sealed partial class SettingsService : SettingsBase, ISingletonService
{
    private bool _isAutoUpdateEnabled;
    private SukiColor _color = SukiColor.Blue;

    public SettingsService(string settingsFilePath)
        : base(settingsFilePath, JsonContext.Default.Options) { }

    public bool IsAutoUpdateEnabled
    {
        get => _isAutoUpdateEnabled;
        set => SetProperty(ref _isAutoUpdateEnabled, value);
    }

    public SukiColor Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

    [JsonSerializable(typeof(SettingsService))]
    [JsonSourceGenerationOptions(
        WriteIndented = true,
        AllowTrailingCommas = true,
        UseStringEnumConverter = true
    )]
    private sealed partial class JsonContext : JsonSerializerContext;
}
