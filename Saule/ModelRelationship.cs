using System;

namespace Saule
{
    public class ModelRelationship
    {
        public ModelRelationship(string name, RelationshipType relationshipType, Type modelType, string urlPath)
        {
            Name = name;
            RelationshipType = RelationshipType;
            ModelType = modelType;
            UrlPath = urlPath;
        }

        public string Name { get; }
        public RelationshipType RelationshipType { get; }
        public Type ModelType { get; }
        public string UrlPath { get; }
    }
}