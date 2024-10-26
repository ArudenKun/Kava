using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using JetBrains.Annotations;

namespace Kava.Services;

[PublicAPI]
public class FileAccessService
{
    private readonly IStorageProvider _storageProvider;

    public FileAccessService(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public Task<IReadOnlyList<IStorageFile>> OpenFileAsync(
        Action<FilePickerOpenOptions>? optionsConfig = null
    )
    {
        var options = new FilePickerOpenOptions();
        optionsConfig?.Invoke(options);
        return _storageProvider.OpenFilePickerAsync(options);
    }

    public Task<IReadOnlyList<IStorageFolder>> OpenFolderAsync(
        Action<FolderPickerOpenOptions>? optionsConfig = null
    )
    {
        var options = new FolderPickerOpenOptions();
        optionsConfig?.Invoke(options);
        return _storageProvider.OpenFolderPickerAsync(options);
    }
}
