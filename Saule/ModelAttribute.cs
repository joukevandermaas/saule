namespace Saule
{
    public class ModelAttribute
    {
        public ModelAttribute(string name)
        {
            Name = name.ToCamelCase();
        }

        public string Name { get; }
    }
}