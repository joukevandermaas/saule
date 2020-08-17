namespace Tests.Models.Inheritance
{
    public class CircleResource : ShapeResource
    {
        public CircleResource()
        {
            Attribute(nameof(Circle.Radius));
        }
    }
}