using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace TaskManager.Utils;

internal static class StringExtensions
{
  [Pure]
  public static bool IsNotNullOrSpaces([NotNullWhen(true)] this string? str)
  {
    return string.IsNullOrWhiteSpace(str) == false;
  }
}