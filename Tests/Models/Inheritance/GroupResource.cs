using Saule;

namespace Tests.Models.Inheritance
{
    public class GroupResource : ApiResource
    {
        public GroupResource()
        {
            WithId(nameof(Group.Id));
            Attribute(nameof(Group.Name));
            HasMany<ShapeResource>(nameof(Group.Shapes));
        }
    }
}