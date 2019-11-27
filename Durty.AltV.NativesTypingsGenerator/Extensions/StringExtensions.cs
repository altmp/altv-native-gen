using System;
using System.Linq;

namespace Durty.AltV.NativesTypingsGenerator.Extensions
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };

        public static string ReplaceFirst(this string text, string search, string replace, StringComparison stringComparison = StringComparison.Ordinal)
        {
            int pos = text.IndexOf(search, stringComparison);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}