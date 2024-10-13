using Avalonia.Interactivity;
using Kava.ViewModels.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kava.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
    private readonly ILogger<MainWindowViewModel> _logger;

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger)
    {
        _logger = logger;

        Loaded += args =>
        {
            _logger.LogInformation("Loaded MainWindowViewModel");
        };

        AttachedToVisualTree += args =>
        {
            logger.LogInformation("AttachedToVisualTree MainWindowViewModel");
        };
    }
}
