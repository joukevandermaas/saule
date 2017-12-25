namespace Saule.Queries.Sorting
{
    /// <summary>
    /// Property for sorting
    /// </summary>
    public class SortingProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortingProperty"/> class.
        /// </summary>
        /// <param name="value">property name and direction</param>
        public SortingProperty(string value)
        {
            Direction = FindSortingDirection(value);
            Name = StripSyntax(value).ToPascalCase();
        }

        /// <summary>
        /// Gets or Sets name of property
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or Sets direction of sorting
        /// </summary>
        public SortingDirection Direction { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Direction == SortingDirection.Descending ? "-" + Name : Name;
        }

        private static string StripSyntax(string value)
        {
            return value.Trim('+', '-').Trim();
        }

        private static SortingDirection FindSortingDirection(string value)
        {
            return value.StartsWith("-")
                ? SortingDirection.Descending
                : SortingDirection.Ascending;
        }
    }
}