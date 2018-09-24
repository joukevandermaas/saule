using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries.Fieldset
{
    /// <summary>
    /// Context for fieldset operations
    /// </summary>
    public class FieldsetContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsetContext"/> class.
        /// </summary>
        /// <param name="queryParams">query string that might contain Fieldset keyword</param>
        public FieldsetContext(IEnumerable<KeyValuePair<string, string>> queryParams)
        {
            Properties =
                from query in queryParams
                where query.Key.StartsWith(Constants.QueryNames.Fieldset)
                let type = query.Key.Substring(Constants.QueryNames.Fieldset.Length + 1)
                let fields = query.Value.Split(',')
                select new FieldsetProperty(type, fields);
        }

        /// <summary>
        /// Gets including properties
        /// </summary>
        public IEnumerable<FieldsetProperty> Properties { get; internal set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Properties != null && Properties.Any() ? string.Join("&", Properties.Select(p => p.ToString())) : string.Empty;
        }
    }
}
