using System;
#if !NETSTANDARD2_0
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
#endif

namespace StripeApiWrapper.Internal;

/// <summary>
/// Helper methods for argument validation that work across all target frameworks.
/// </summary>
internal static class ThrowHelper
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the argument is null.
    /// </summary>
#if NETSTANDARD2_0
    public static void ThrowIfNull(object? argument, string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
#else
    public static void ThrowIfNull(
        [NotNull] object? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
    }
#endif

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the argument is null or whitespace.
    /// </summary>
#if NETSTANDARD2_0
    public static void ThrowIfNullOrWhiteSpace(string? argument, string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
        }
    }
#else
    public static void ThrowIfNullOrWhiteSpace(
        [NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument, paramName);
    }
#endif
}
