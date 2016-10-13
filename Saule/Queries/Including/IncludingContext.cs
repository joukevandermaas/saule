using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saule.Queries.Including
{
    internal class IncludingContext
    {
        public IncludingContext(IEnumerable<KeyValuePair<string, string>> includes)
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

        public IEnumerable<IncludingProperty> Includes { get; }
    }
}
