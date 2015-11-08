using System;

namespace Tests.Models
{
    internal class GuidAsId
    {
        public GuidAsId(Guid id = default(Guid))
        {
            if (id != default(Guid))
                Id = id;
        }

        public Guid Id { get; } = Guid.NewGuid();
    }
}
