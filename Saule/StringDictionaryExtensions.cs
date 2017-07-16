using System;
using System.Collections.Generic;

namespace Saule
{
    /// <summary>
    /// Extension methods for operating on IDictionary[string, string]
    /// </summary>
    internal static class StringDictionaryExtensions
    {
        /// <summary>
        /// Parse an integer value from a dictionary.
        /// </summary>
        /// <param name="dictionary">Dictionary being interrogated</param>
        /// <param name="key">Key for the value to be parsed</param>
        /// <returns>Int32 parsed from dictionary value</returns>
        public static int? GetInt(this IDictionary<string, string> dictionary, string key)
        {
            string keyValue;
            dictionary.TryGetValue(key, out keyValue);
            int intValue;
            bool legit = int.TryParse(keyValue, out intValue);
            return legit ? intValue : (int?)null;
        }

        /// <summary>
        /// Parse an integer value from a dictionary.
        /// </summary>
        /// <param name="dictionary">Dictionary being interrogated</param>
        /// <param name="key">Key for the value to be parsed</param>
        /// <param name="defaultValue">Default value for unparsable keys</param>
        /// <returns>Int32 parsed from dictionary value</returns>
        public static int GetInt(this IDictionary<string, string> dictionary, string key, int defaultValue)
        {
            return GetInt(dictionary, key) ?? defaultValue;
        }
    }
}
