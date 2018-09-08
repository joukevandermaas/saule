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
        /// <param name="name">property name</param>
        public FieldsetProperty(string type, string[] fields)
        {
            Type = type;
            Fields = fields;
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
            return $"filter[{Type}]={string.Join(",", Fields)}";
        }
    }
}