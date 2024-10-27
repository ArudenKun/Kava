using System;

namespace Kava.Core.Utilities;

public sealed class Disposable(Action dispose) : IDisposable
{
    public static IDisposable Create(Action dispose) => new Disposable(dispose);

    public void Dispose() => dispose();
}
