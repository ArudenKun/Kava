using CommunityToolkit.Mvvm.ComponentModel;
using Kava.ViewModels.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kava.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
    private readonly ILogger<MainWindowViewModel> _logger;

    [ObservableProperty]
    private string _pdfPath = "";

    [ObservableProperty]
    private bool _isIdle;

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger)
    {
        _logger = logger;

        Loaded += () =>
        {
            _logger.LogInformation("Loaded MainWindowViewModel");
            PdfPath = @"C:\Users\alden\Downloads\Dada-Flight-Dec17-2024-MNL-CGO.pdf";
        };

        Unloaded += () =>
        {
            _logger.LogInformation("UnLoaded MainWindowViewModel");
        };
    }

    partial void OnIsIdleChanged(bool value)
    {
        if (!value)
        {
            return;
        }

        _logger.LogInformation("User is idle on: {Time}", DateTimeOffset.Now);
        ShowIdleDialog();
    }

    private void ShowIdleDialog() { }
}
