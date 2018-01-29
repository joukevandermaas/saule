namespace Saule.Queries.Sorting
{
    /// <summary>
    /// Property for sorting
    /// </summary>
    public class SortProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortProperty"/> class.
        /// </summary>
        /// <param name="value">property name and direction</param>
        public SortProperty(string value)
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
        public SortDirection Direction { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Direction == SortDirection.Descending ? "-" + Name : Name;
        }

        private static string StripSyntax(string value)
        {
            return value.Trim('+', '-').Trim();
        }

        private static SortDirection FindSortingDirection(string value)
        {
            return value.StartsWith("-")
                ? SortDirection.Descending
                : SortDirection.Ascending;
        }
    }
}