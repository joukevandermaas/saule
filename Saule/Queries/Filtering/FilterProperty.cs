using System;
using System.Collections.Generic;
using System.Linq;
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

            // If there is no comma we add the whole string as one value
            if (!values.Contains(','))
            {
                Values.Add(values);
                return;
            }

            // Check for a multiple values in case of quotes
            // If the string does start with a quote it has to have an end quote followed by a comma.
            bool multipleValues = !(values.StartsWith("\"") && !values.Contains("\","));

            if (!multipleValues)
            {
                Values.Add(values);
                return;
            }

            /*
             * This Regex contains two matching groups
             *
             * Regex in plain: (?:,"|^")(""|[\w\W]*?)(?=",|"$)|(?:,(?!")|^(?!"))([^,]*?)(?=$|,)
             *
             * 1. (?:,"|^")(""|[\w\W]*?)(?=",|"$)
             * It starts of with a non capturing group on commas followed by quotes or just
             * quotes at the beginning. Then we will look for as few words as possible until the next
             * quotes followed by a comma or the end of the string. This will handle all the cases of
             * values enclosed by quotes.
             *
             * 2. (?:,(?!")|^(?!"))([^,]*?)(?=$|,)
             * This starts of with a big non capturing group. Either a comma NOT followed by quotes or
             * no quotes at the start of the string. Then it will match until the next comma is is reached.
             * The last part again is to make sure it ends with either a comma or the end of the string.
             * This group handles all cases with simple comma separation.
             */
            var match = new Regex("(?:,\"|^\")(\"\"|[\\w\\W]*?)(?=\",|\"$)|(?:,(?!\")|^(?!\"))([^,]*?)(?=$|,)").Matches(values);
            foreach (Match matchCapture in match)
            {
                var captureValue = string.IsNullOrEmpty(matchCapture.Groups[1].Value) ? matchCapture.Groups[2].Value : matchCapture.Groups[1].Value;

                // We need to check if it is empty because capture group 2 will have some empty results e.g.for a trailing comma.
                if (!string.IsNullOrEmpty(captureValue))
                {
                    Values.Add(captureValue);
                }
            }

            // If there were no matches we have a malformed csv and we just add it as one filter
            if (Values.Count == 0)
            {
                Values.Add(values);
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