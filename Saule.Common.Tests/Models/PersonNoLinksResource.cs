using Saule;

namespace Tests.Models
{
    public class PersonNoLinksResource : ApiResource
    {
        public PersonNoLinksResource()
        {
            WithId(nameof(Person.Identifier));
            WithLinks(LinkType.None);

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