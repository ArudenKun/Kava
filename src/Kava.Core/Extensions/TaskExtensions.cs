using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Kava.Core.Extensions;

public static class TaskExtensions
{
    /// <summary>
    ///     Run the specified <see cref="Action"/> with <paramref name="curr"/> passed in as its value,
    ///     then return <paramref name="curr"/>.
    /// </summary>
    /// <param name="curr">The current object.</param>
    /// <param name="apply">The action to perform.</param>
    /// <typeparam name="T">The type of the current object.</typeparam>
    /// <returns><paramref name="curr"/> after <paramref name="apply"/> runs on it.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Apply<T>(this T curr, Action<T> apply)
    {
        apply(curr);
        return curr;
    }

    /// <summary>
    ///     Run the specified <see cref="Func{TResult}"/> with <paramref name="curr"/> passed in as its value,
    ///     then return <paramref name="curr"/>.
    /// </summary>
    /// <param name="curr">The current object.</param>
    /// <param name="applyAsync">The async action to perform.</param>
    /// <typeparam name="T">The type of the current object.</typeparam>
    /// <returns><paramref name="curr"/> after <paramref name="applyAsync"/> runs on it.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<T> ApplyAsync<T>(this T curr, Func<T, Task> applyAsync)
    {
        await applyAsync(curr).ConfigureAwait(false);
        return curr;
    }
}
