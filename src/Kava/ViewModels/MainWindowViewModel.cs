using CommunityToolkit.Mvvm.ComponentModel;
using Kava.ViewModels.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kava.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
    private readonly ILogger<MainWindowViewModel> _logger;

    [ObservableProperty]
    private string _pdfPath = @"C:\Users\alden\Downloads\Dada-Flight-Dec17-2024-MNL-CGO.pdf";

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger)
    {
        _logger = logger;

        Loaded += (_, _) =>
        {
            _logger.LogInformation("Loaded MainWindowViewModel");
            PdfPath = @"C:\Users\alden\Downloads\Dada-Flight-Dec17-2024-MNL-CGO.pdf";
        };

        AttachedToVisualTree += (_, _) =>
        {
            logger.LogInformation("AttachedToVisualTree MainWindowViewModel");
        };
    }
}
