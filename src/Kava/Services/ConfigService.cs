using System;
using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using Kava.Core;
using Microsoft.Extensions.Logging;
using SukiUI.Enums;

namespace Kava.Services;

[INotifyPropertyChanged]
public sealed partial class ConfigService : SettingsBase, IDisposable
{
    private readonly ILogger<ConfigService> _logger;

    private bool _isAutoUpdateEnabled;
    private SukiColor _color = SukiColor.Blue;

    /// <inheritdoc/>
    public ConfigService(ILogger<ConfigService> logger)
        : base(AppInfo.ConfigPath.Path, AppJsonContext.Default.Options)
    {
        _logger = logger;
    }

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

    public override bool Load()
    {
        _logger.LogInformation("Loading Config");
        var loaded = base.Load();
        if (loaded)
        {
            _logger.LogInformation("Loaded Config");
            return true;
        }

        _logger.LogWarning("Failed to load Config, Using Default Config Values");
        Reset();
        return false;
    }

    public override void Save()
    {
        _logger.LogInformation("Saving Config");
        base.Save();
        _logger.LogInformation("Saved Config");
    }

    public void Dispose() => Save();
}
