using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Serilog;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Kava.Utilities.Helpers;

// ReSharper disable once InconsistentNaming
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class OSNativeHelper
{
    private static readonly ILogger Logger;

    static OSNativeHelper()
    {
        Logger = Log.ForContext(typeof(OSNativeHelper));
    }

    [SupportedOSPlatform("windows")]
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public static class Windows
    {
        internal static MESSAGEBOX_RESULT ErrorMessageBox(string caption, string text) =>
            PInvoke.MessageBox((HWND)0, text, caption, MESSAGEBOX_STYLE.MB_ICONERROR);

        public static bool SetConsoleWindowStateSupported => OperatingSystem.IsWindows();

        public static void SetConsoleWindowState(bool show)
        {
            if (OperatingSystem.IsWindows())
            {
                SetConsoleWindowStateWindows(show);
            }
            else if (show == false)
            {
                Logger.Error("OS doesn't support hiding console window");
            }
        }

        [SupportedOSPlatform("windows")]
        private static void SetConsoleWindowStateWindows(bool show)
        {
            HWND hWnd = PInvoke.GetConsoleWindow();

            if (hWnd == IntPtr.Zero)
            {
                Logger.Warning(
                    "Attempted to show/hide console window but console window does not exist"
                );
                return;
            }

            _ = PInvoke.ShowWindow(hWnd, show ? SHOW_WINDOW_CMD.SW_SHOW : SHOW_WINDOW_CMD.SW_HIDE);
        }
    }
}
