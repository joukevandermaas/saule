using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Saule.Queries.Filtering
{
    /// <summary>
    /// Property for filtering
    /// </summary>
    public class FilterProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterProperty"/> class.
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="values">property values in one string in csv notation</param>
        public FilterProperty(string name, string values)
        {
            Name = name.ToPascalCase();
            Value = values;

            // Spliting the string into multiple values with csv notation
            Values = new List<string>();

            var match = new Regex("\"(.+?)\"|(\\w+(?=,|$))").Matches(values);
            foreach (Capture matchCapture in match)
            {
                var captureValue = matchCapture.Value;

                // Fix for regex matching including the double quotes
                captureValue = captureValue.StartsWith("\"") && captureValue.EndsWith("\"")
                    ? captureValue.Substring(1, captureValue.Length - 2)
                    : captureValue;

                Values.Add(captureValue);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterProperty"/> class.
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="values">property values</param>
        public FilterProperty(string name, List<string> values)
        {
            Name = name.ToPascalCase();
            Values = values;
            Value = string.Join(",", values);
        }

        /// <summary>
        /// Gets property name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets property value
        /// </summary>
        public List<string> Values { get; }

        /// <summary>
        /// Gets property string value
        /// </summary>
        public string Value { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"filter[{Name}]={Value}";
        }
    }
}