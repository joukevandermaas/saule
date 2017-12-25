using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries.Including
{
    /// <summary>
    /// Context for including operations
    /// </summary>
    public class IncludingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncludingContext"/> class.
        /// </summary>
        public IncludingContext()
        {
            DisableDefaultIncluded = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncludingContext"/> class.
        /// </summary>
        /// <param name="includes">query string that might contain Include keyword</param>
        public IncludingContext(IEnumerable<KeyValuePair<string, string>> includes)
            : this()
        {
            SetIncludes(includes);
        }

        /// <summary>
        /// Gets including properties
        /// </summary>
        public IEnumerable<IncludingProperty> Includes { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether includes shouldn't be returned by default or not
        /// </summary>
        public bool DisableDefaultIncluded { get; internal set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Includes != null && Includes.Any() ? "include=" + string.Join(",", Includes.Select(p => p.ToString())) : string.Empty;
        }

        internal void SetIncludes(IEnumerable<KeyValuePair<string, string>> includes)
        {
            var dict = includes.ToDictionary(kv => kv.Key, kv => kv.Value);
            if (dict.ContainsKey(Constants.QueryNames.Including))
            {
                var relatedResources = dict[Constants.QueryNames.Including].Split(',').Select(s => s.Trim());
                Includes = relatedResources.Select(x => new IncludingProperty(x));
            }
            else
            {
                Includes = new IncludingProperty[0];
            }
        }
    }
}
