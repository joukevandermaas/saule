using Saule;

namespace Tests.Models
{
    public class PersonJobOnlyRelatedLinksResource : ApiResource
    {
        public PersonJobOnlyRelatedLinksResource()
        {
            WithId(nameof(Person.Identifier));

            Attribute(nameof(Person.FirstName));
            Attribute(nameof(Person.LastName));
            Attribute(nameof(Person.Age));
            Attribute(nameof(Person.Address));

            BelongsTo<CompanyResource>(nameof(Person.Job), "/employer", LinkType.Related);
            BelongsTo<CarResource>(nameof(Person.Car));
            HasMany<PersonResource>(nameof(Person.Friends));
            HasMany<PersonResource>( nameof( Person.FamilyMembers));
        }
    }
}