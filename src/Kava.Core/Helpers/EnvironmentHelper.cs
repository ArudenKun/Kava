using System;
using System.Globalization;
using System.Reflection;

namespace Kava.Core.Helpers;

public static class EnvironmentHelper
{
    public static Assembly TargetAssembly { get; set; } = Assembly.GetExecutingAssembly();

    /// <summary>
    ///     Returns the version of executing assembly.
    /// </summary>
    public static Version AppVersion => TargetAssembly.GetName().Version ?? new Version();

    /// <summary>
    ///     Returns the friendly name of this application.
    /// </summary>
    public static string AppName
    {
        get
        {
            var name = AppDomain.CurrentDomain.FriendlyName;
            var split = name.Split('.');
            return split.Length > 1 ? split[0] : name;
        }
    }

    /// <summary>
    ///     Returns the path of the ApplicationData.
    /// </summary>
    public static string AppDataDirectory => RoamingDirectory.JoinPath(AppName);

    /// <summary>
    ///     Returns the directory from which the application is run.
    /// </summary>
    public static string AppDirectory => AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    ///     Returns the path of the roaming directory.
    /// </summary>
    public static string RoamingDirectory =>
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    /// <summary>
    ///     Returns the directory of the user directory (ex: C:\Users\[the name of the user])
    /// </summary>
    public static string UserDirectory =>
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    /// <summary>
    ///     Returns the directory of the downloads directory
    /// </summary>
    public static string DownloadsDirectory => UserDirectory.JoinPath("Downloads");

    /// <summary>
    ///     Gets or sets the <see cref="T:System.Globalization.CultureInfo" /> object that represents the culture used by the
    ///     current thread and task-based asynchronous operations.
    /// </summary>
    public static IFormatProvider CurrentCulture => CultureInfo.CurrentCulture;

    public static bool IsDebug
#if DEBUG
        => true;
#else
        => false;
#endif
}
