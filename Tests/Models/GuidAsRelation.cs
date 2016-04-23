using System.Collections.Generic;

namespace Tests.Models
{
    internal class GuidAsRelation
    {
        public GuidAsRelation(string id = "938")
        {
            Id = id;
            Relation = new GuidAsId();
            Relations = new [] { new GuidAsId(), new GuidAsId() };
        }

        public string Id { get; set; }

        public GuidAsId Relation { get; set; }

        public IEnumerable<GuidAsId> Relations {get; set;}
    }
}
