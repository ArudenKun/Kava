﻿using System;
using System.Collections.Generic;
using Kava.Core.Utilities;

namespace Kava.Core.Extensions;

public static class DisposableExtensions
{
    public static void DisposeAll(this IEnumerable<IDisposable> disposables)
    {
        var exceptions = default(List<Exception>);

        foreach (var disposable in disposables)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception ex)
            {
                (exceptions ??= []).Add(ex);
            }
        }

        if (exceptions?.Count > 0)
            throw new AggregateException(exceptions);
    }

    /// <summary>
    /// Ensures the provided disposable is disposed with the specified <see cref="DisposableCollector"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the disposable.
    /// </typeparam>
    /// <param name="disposable">
    /// The disposable we are going to want to be disposed by the CompositeDisposable.
    /// </param>
    /// <param name="disposableCollector">
    /// The <see cref="DisposableCollector"/> to which <paramref name="disposable"/> will be added.
    /// </param>
    /// <returns>
    /// The disposable.
    /// </returns>
    public static T DisposeWith<T>(this T disposable, DisposableCollector disposableCollector)
        where T : IDisposable
    {
        disposableCollector.Add(disposable);
        return disposable;
    }
}
