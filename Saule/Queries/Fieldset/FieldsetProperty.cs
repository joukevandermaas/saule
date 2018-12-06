using System.Linq;

namespace Saule.Queries.Fieldset
{
    /// <summary>
    /// Property for fieldset
    /// </summary>
    public class FieldsetProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsetProperty"/> class.
        /// </summary>
        /// <param name="type">type for field filter</param>
        /// <param name="fields">fields to serialize filter</param>
        public FieldsetProperty(string type, string[] fields)
        {
            Type = type;
            Fields = fields.Select(f => f.ToComparablePropertyName()).ToArray();
        }

        /// <summary>
        /// Gets property type
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets property fields
        /// </summary>
        public string[] Fields { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"fields[{Type}]={string.Join(",", Fields)}";
        }
    }
}