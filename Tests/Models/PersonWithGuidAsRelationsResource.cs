using Saule;

namespace Tests.Models
{
    public class PersonWithGuidAsRelationsResource : ApiResource
    {
        public PersonWithGuidAsRelationsResource()
        {
            WithId(nameof(GuidAsRelation.Id));

            BelongsTo<PersonWithDefaultIdResource>(nameof(GuidAsRelation.Relation));

            HasMany<PersonWithDefaultIdResource>(nameof(GuidAsRelation.Relations));
        }
    }
}