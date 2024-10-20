using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Kava.Services;
using Kava.Services.Abstractions;
using Kava.Utilities.Extensions.Avalonia;
using Kava.ViewModels.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kava.ViewModels;

public partial class MainWindowViewModel : BaseViewModel, ISingleton
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly DataService _dataService;
    private readonly StorageService _storageService;

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        DataService dataService,
        StorageService storageService
    )
    {
        _logger = logger;
        _dataService = dataService;
        _storageService = storageService;
    }

    [RelayCommand]
    private async Task OpenPdf()
    {
        var storageFiles = await _storageService.OpenFileAsync(options =>
        {
            options.AllowMultiple = false;
        });

        foreach (var storageFile in storageFiles)
        {
            _logger.LogInformation($"Opening file: {storageFile.Name}");
        }

        await storageFiles[0].LaunchFileAsync();
    }
}
