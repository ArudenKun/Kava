using Kava.Hosting.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kava.Hosting.Services.Internal;

/// <summary>
/// This is the configuration for the mutex service
/// </summary>
internal class MutexBuilder : IMutexBuilder
{
    /// <inheritdoc />
    public string MutexId { get; set; } = string.Empty;

    /// <inheritdoc />
    public bool IsGlobal { get; set; }

    /// <inheritdoc />
    public Action<IHostEnvironment, ILogger>? WhenNotFirstInstance { get; set; }
}
