namespace Saule.Queries.Filtering
{
    /// <summary>
    /// Property for filtering
    /// </summary>
    public class FilteringProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringProperty"/> class.
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        public FilteringProperty(string name, string value)
        {
            Value = value;
            Name = name.ToPascalCase();
        }

        /// <summary>
        /// Gets property name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets property value
        /// </summary>
        public string Value { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"filter[{Name}]={Value}";
        }
    }
}