using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform.Storage;

namespace Kava.Utilities.Extensions.Avalonia;

public static class StorageExtensions
{
    private static ILauncher Launcher => Application.Current!.GetLauncher();

    public static Task LaunchFileAsync(this IStorageFile file) => Launcher.LaunchFileAsync(file);
}
