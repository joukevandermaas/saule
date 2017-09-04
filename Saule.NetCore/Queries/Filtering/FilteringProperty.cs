namespace Saule.Queries.Filtering
{
    internal class FilteringProperty
    {
        public FilteringProperty(string name, string value)
        {
            Value = value;
            Name = name.ToPascalCase();
        }

        public string Name { get; }

        public string Value { get; }

        public override string ToString()
        {
            return $"filter[{Name}]={Value}";
        }
    }
}