using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Saule
{
    internal static class StringExtensions
    {
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

            return string.Join("", parts.Select(s =>
                CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s)).ToArray());
        }

        public static string ToCamelCase(this string source)
        {
            // someString
            var parts = SplitAndLower(source);
            var firstWord = parts.FirstOrDefault()?.ToLower();

            var cased = parts.Select((s, i) =>
            {
                if (i == 0)
                    return s.ToLowerInvariant();
                else
                    return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s);
            });

            return string.Join("", cased.ToArray());
        }

        private static IEnumerable<string> SplitAndLower(string source)
        {
            var strings = new List<string>();
            var builder = new StringBuilder();

            for (int i = 0; i < source.Length; i++)
            {
                if (IsSeparator(source[i]))
                {
                    if (builder.Length > 0)
                        strings.Add(builder.ToString());
                    builder.Clear();
                    continue;
                }
                else if (char.IsUpper(source[i]) && builder.Length > 0)
                {
                    strings.Add(builder.ToString());
                    builder.Clear();
                }

                builder.Append(char.ToLowerInvariant(source[i]));
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