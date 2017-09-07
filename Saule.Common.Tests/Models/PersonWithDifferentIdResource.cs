using Saule;

namespace Tests.Models
{
    public class PersonWithDifferentIdResource : ApiResource
    {
        public PersonWithDifferentIdResource()
        {
            OfType("Person");
            WithId(nameof(PersonWithDifferentId.PersonId));

            Attribute(nameof(PersonWithDifferentId.FirstName));
            Attribute(nameof(PersonWithDifferentId.LastName));
            Attribute(nameof(PersonWithDifferentId.Age));

            BelongsTo<CompanyWithDifferentIdResource>(nameof(PersonWithDifferentId.Job), "/employer");
            HasMany<PersonWithDifferentIdResource>(nameof(PersonWithDifferentId.Friends));
        }
    }
}