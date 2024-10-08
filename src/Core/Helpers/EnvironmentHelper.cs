﻿using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Flurl;

namespace Core.Helpers;

public static class EnvironmentHelper
{
    /// <summary>
    ///     Returns the version of executing assembly.
    /// </summary>
    public static Version AppVersion =>
        Assembly.GetExecutingAssembly().GetName().Version ?? new Version();

    /// <summary>
    ///     Returns the friendly name of this application.
    /// </summary>
    public static string AppName => AppDomain.CurrentDomain.FriendlyName;

    /// <summary>
    ///     Returns the path of the ApplicationData.
    /// </summary>
    public static string AppDataDirectory =>
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).JoinPath(AppName);

    /// <summary>
    ///     Returns the directory from which the application is run.
    /// </summary>
    public static string AppDirectory => AppDomain.CurrentDomain.BaseDirectory;

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

    public static void OpenUrl(Url url)
    {
        if (OperatingSystemHelper.IsWindows)
        {
            Process.Start(
                new ProcessStartInfo(url.ToString().Replace("&", "^&")) { UseShellExecute = true }
            );
            return;
        }

        Process.Start("xdg-open", url);
    }
}
