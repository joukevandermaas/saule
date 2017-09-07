namespace Saule.Queries.Including
{
    internal class IncludingProperty
    {
        public IncludingProperty(string name)
        {
            Name = name.ToPascalCase();
        }

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}