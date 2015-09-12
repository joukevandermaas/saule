using System;

namespace Saule
{
    public class ResourceRelationship
    {
        public ResourceRelationship(string name, RelationshipType relationshipType, Type modelType, string urlPath)
        {
            Name = name.ToCamelCase();
            RelationshipType = RelationshipType;
            ModelType = modelType;
            UrlPath = urlPath.ToDashed();
        }

        public string Name { get; }
        public RelationshipType RelationshipType { get; }
        public Type ModelType { get; }
        public string UrlPath { get; }
    }
}