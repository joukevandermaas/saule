using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries.Including
{
    public class IncludingContext
    {
        public IncludingContext()
        {
            DisableDefaultIncluded = false;
        }

        public IncludingContext(IEnumerable<KeyValuePair<string, string>> includes)
            : this()
        {
            SetIncludes(includes);
        }

        public IEnumerable<IncludingProperty> Includes { get; internal set; }

        public bool DisableDefaultIncluded { get; internal set; }

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
