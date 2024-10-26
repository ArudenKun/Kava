using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using Kava.Utilities.Helpers;
using SukiUI.Enums;

namespace Kava.Services;

[INotifyPropertyChanged]
public sealed partial class SettingsService : SettingsBase
{
    private bool _isAutoUpdateEnabled;
    private SukiColor _color = SukiColor.Blue;

    public SettingsService()
        : base(
            EnvironmentHelper.AppDataDirectory.JoinPath("settings.json"),
            KavaJsonContext.Default.Options
        ) { }

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
