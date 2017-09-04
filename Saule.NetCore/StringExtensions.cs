using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Saule
{
    internal static class StringExtensions
    {
        public static string EnsureEndsWith(this string source, string end)
        {
            return source.EndsWith(end)
                ? source
                : source + end;
        }

        public static string EnsureStartsWith(this string source, string start)
        {
            return source.StartsWith(start)
                ? source
                : start + source;
        }

        public static string TrimJoin(this char separator, params string[] parts)
        {
            return string.Join(separator.ToString(), parts.Select(p => p.Trim(separator)));
        }

        public static string ToDashed(this string source)
        {
            // some-string
            var parts = SplitAndLower(source);

            return string.Join("-", parts.ToArray());
        }

        public static string ToPascalCase(this string source)
        {
            // SomeString
            var parts = SplitAndLower(source);

            return string.Join(
                string.Empty,
                parts.Select(s => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s)).ToArray());
        }

        public static string ToCamelCase(this string source)
        {
            // someString
            var parts = SplitAndLower(source);

            var cased = parts.Select((s, i) =>
                i == 0
                    ? s.ToLowerInvariant()
                    : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s));

            return string.Join(string.Empty, cased.ToArray());
        }

        public static string SubstringToSeperator(this string source, string seperator)
        {
            var to = source.IndexOf(seperator);
            return to != -1 ? source.Substring(0, to) : source;
        }

        private static IEnumerable<string> SplitAndLower(string source)
        {
            var strings = new List<string>();
            var builder = new StringBuilder();

            foreach (var character in source)
            {
                if (IsSeparator(character))
                {
                    if (builder.Length > 0)
                    {
                        strings.Add(builder.ToString());
                    }

                    builder.Clear();
                    continue;
                }
                else if (char.IsUpper(character) && builder.Length > 0)
                {
                    strings.Add(builder.ToString());
                    builder.Clear();
                }

                builder.Append(char.ToLowerInvariant(character));
            }

            if (builder.Length > 0)
            {
                strings.Add(builder.ToString());
            }

            return strings;
        }

        private static bool IsSeparator(char value)
        {
            return value == '-' || value == '_' || value == ' ';
        }
    }
}