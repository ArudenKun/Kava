using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Kava.Models;
using Kava.Services;
using Kava.ViewModels.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kava.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly FileAccessService _fileAccessService;
    private readonly ILauncher _launcher;
    private readonly IFreeSql _freeSql;

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        FileAccessService fileAccessService,
        ILauncher launcher,
        IFreeSql freeSql
    )
    {
        _logger = logger;
        _fileAccessService = fileAccessService;
        _launcher = launcher;
        _freeSql = freeSql;
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

    protected override async void OnLoaded()
    {
        var salt = $"salt:{Random.Shared.Next()}";

        var board = new Board { Name = $"Board Name {salt}" };
        await _freeSql.Insert(board).ExecuteAffrowsAsync();
        var card = new Card
        {
            Name = $"Card Name {salt}",
            Description = $"Card Description {salt}",
            BoardId = board.Id,
        };
        await _freeSql.Insert(card).ExecuteAffrowsAsync();
        var entry = new Entry
        {
            Summary = $"Entry Summary {salt}",
            Description = $"Entry Description {salt}",
            CardId = card.Id,
        };
        await _freeSql.Insert(entry).ExecuteAffrowsAsync();
        var attachment = new Attachment
        {
            FileName = $"Attachment-{salt}.png",
            Size = 200000,
            EntryId = entry.Id,
        };
        await _freeSql.Insert(attachment).ExecuteAffrowsAsync();
    }
}
