namespace Saule
{
    public class ResourceAttribute
    {
        public ResourceAttribute(string name)
        {
            Name = name.ToCamelCase();
        }

        public string Name { get; }
    }
}