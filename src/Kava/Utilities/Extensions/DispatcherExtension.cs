using System.Threading.Tasks;
using Avalonia.Threading;

namespace Kava.Utilities.Extensions;

public static class DispatcherExtension
{
    public static void WaitOnDispatcherFrame(this Task task, Dispatcher? dispatcher = null)
    {
        var frame = new DispatcherFrame();
        AggregateException? capturedException = null;

        task.ContinueWith(
            t =>
            {
                capturedException = t.Exception;
                frame.Continue = false; // 结束消息循环
            },
            TaskContinuationOptions.AttachedToParent
        );

        dispatcher ??= Dispatcher.UIThread;
        dispatcher.PushFrame(frame);

        if (capturedException != null)
        {
            throw capturedException;
        }
    }
}
