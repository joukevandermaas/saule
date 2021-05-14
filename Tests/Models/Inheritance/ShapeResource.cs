using Saule;

namespace Tests.Models.Inheritance
{
    public class ShapeResource : ApiResource
    {
        public ShapeResource()
        {
            WithId(nameof(Shape.Id));

            Attribute(nameof(Shape.Color));
        }
    }
}