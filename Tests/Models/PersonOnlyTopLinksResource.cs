using Saule;

namespace Tests.Models
{
    public class PersonOnlyTopLinksResource : ApiResource
    {
        public PersonOnlyTopLinksResource()
        {
            WithId(nameof(Person.Identifier));
            WithLinks(LinkType.Top);

            Attribute(nameof(Person.FirstName));
            Attribute(nameof(Person.LastName));
            Attribute(nameof(Person.Age));
            Attribute(nameof(Person.Address));

            BelongsTo<CompanyResource>(nameof(Person.Job), "/employer");
            BelongsTo<CarResource>(nameof(Person.Car));
            HasMany<PersonResource>(nameof(Person.Friends));
            HasMany<PersonResource>( nameof( Person.FamilyMembers));
        }
    }
}