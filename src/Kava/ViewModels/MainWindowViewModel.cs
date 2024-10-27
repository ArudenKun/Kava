using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Kava.Services;
using Kava.ViewModels.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kava.ViewModels;

public partial class MainWindowViewModel : BaseViewModel, ISingletonViewModel
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly FileAccessService _fileAccessService;
    private readonly ILauncher _launcher;

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        FileAccessService fileAccessService,
        ILauncher launcher
    )
    {
        _logger = logger;
        _fileAccessService = fileAccessService;
        _launcher = launcher;
    }

    [RelayCommand]
    private async Task OpenPdf()
    {
        var storageFiles = await _fileAccessService.OpenFileAsync(options =>
        {
            options.AllowMultiple = false;
        });

        foreach (var storageFile in storageFiles)
        {
            _logger.LogInformation("Opening file: {FileName}", storageFile.Name);
        }

        await _launcher.LaunchFileAsync(storageFiles[0]);
    }
}
