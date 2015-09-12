using System;
using System.Collections.Generic;

namespace Saule
{
    public abstract class ApiResource
    {
        private List<ResourceAttribute> _attributes = new List<ResourceAttribute>();
        private List<ResourceRelationship> _relationships = new List<ResourceRelationship>();

        public IEnumerable<ResourceAttribute> Attributes => _attributes;
        public IEnumerable<ResourceRelationship> Relationships => _relationships;
        public string ResourceType { get; private set; }

        protected ApiResource()
        {
            var name = GetType().Name;
            if(name.ToUpperInvariant().EndsWith("RESOURCE"))
            {
                WithType(name.Remove(name.Length - "RESOURCE".Length));
            } else
            {
                WithType(name);
            }
        }

        protected void WithType(string name)
        {
            ResourceType = name.ToDashed();
        }

        protected void WithAttribute(string name)
        {
            _attributes.Add(new ResourceAttribute(name));
        }

        protected void BelongsTo(string name, Type type)
        {
            BelongsTo(name, type, name);
        }
        protected void BelongsTo(string name, Type type, string path)
        {
            _relationships.Add(new ResourceRelationship(name, RelationshipType.Single, type, path));
        }

        protected void HasMany(string name, Type type)
        {
            HasMany(name, type, name);
        }
        protected void HasMany(string name, Type type, string path)
        {
            _relationships.Add(new ResourceRelationship(name, RelationshipType.Many, type, path));
        }
    }
}
