namespace Saule.Queries.Sorting
{
    internal class SortingProperty
    {
        public SortingProperty(string value)
        {
            Direction = FindSortingDirection(value);
            Name = StripSyntax(value).ToPascalCase();
        }

        public string Name { get; }

        public SortingDirection Direction { get; }

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