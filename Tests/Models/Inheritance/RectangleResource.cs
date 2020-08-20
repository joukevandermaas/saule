namespace Tests.Models.Inheritance
{
    public class RectangleResource : ShapeResource
    {
        public RectangleResource()
        {
            Attribute(nameof(Rectangle.Left));
            Attribute(nameof(Rectangle.Top));
            Attribute(nameof(Rectangle.Width));
            Attribute(nameof(Rectangle.Height));
        }
    }
}