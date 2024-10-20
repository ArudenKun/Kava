using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using Kava.Services.Abstractions;
using SukiUI.Enums;

namespace Kava.Services;

[INotifyPropertyChanged]
public sealed partial class SettingsService : SettingsBase, ISingleton
{
    private bool _isAutoUpdateEnabled;
    private SukiColor _color = SukiColor.Blue;

    public SettingsService(string settingsFilePath)
        : base(settingsFilePath, AppJsonContext.Default.Options) { }

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
}
