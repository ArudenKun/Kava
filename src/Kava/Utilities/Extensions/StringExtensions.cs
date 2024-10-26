using System;
using System.Linq;
using JetBrains.Annotations;

namespace Kava.Utilities.Extensions;

internal static class StringExtensions
{
    private const string AlphanumericChars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static char[] ValidAlphanumerics => AlphanumericChars.ToArray();

    public static string RandomizeSequence(string str, [NonNegativeValue] int rolls = 0)
    {
        var result = Rearrange(str);
        Lambda.Repeat(rolls, () => result = Rearrange(result));
        return result;

        string Rearrange(string s) => string.Join("", s.OrderBy(_ => Guid.NewGuid()));
    }
}
