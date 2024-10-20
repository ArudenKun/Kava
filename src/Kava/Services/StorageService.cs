using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Kava.Services.Abstractions;
using Kava.Utilities.Extensions;
using Kava.Utilities.Extensions.Avalonia;

namespace Kava.Services;

public class StorageService : ISingleton
{
    private static TopLevel TopLevel =>
        Application.Current?.TryGetTopLevel()
        ?? throw new NullReferenceException("TopLevel not found");

    private static IStorageProvider StorageProvider => TopLevel.StorageProvider;

    public Task<IReadOnlyList<IStorageFile>> OpenFileAsync(
        Action<FilePickerOpenOptions>? optionsConfig = null
    )
    {
        var options = new FilePickerOpenOptions();
        optionsConfig?.Invoke(options);
        return StorageProvider.OpenFilePickerAsync(options);
    }

    public Task<IReadOnlyList<IStorageFolder>> OpenFolderAsync(
        Action<FolderPickerOpenOptions>? optionsConfig = null
    )
    {
        var options = new FolderPickerOpenOptions();
        optionsConfig?.Invoke(options);
        return StorageProvider.OpenFolderPickerAsync(options);
    }
}
