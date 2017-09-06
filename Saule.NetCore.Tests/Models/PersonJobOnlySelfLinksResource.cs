using Saule;

namespace Tests.Models
{
    public class PersonJobOnlySelfLinksResource : ApiResource
    {
        public PersonJobOnlySelfLinksResource()
        {
            WithId(nameof(Person.Identifier));

            Attribute(nameof(Person.FirstName));
            Attribute(nameof(Person.LastName));
            Attribute(nameof(Person.Age));
            Attribute(nameof(Person.Address));

            BelongsTo<CompanyResource>(nameof(Person.Job), "/employer", LinkType.Self);
            BelongsTo<CarResource>(nameof(Person.Car));
            HasMany<PersonResource>(nameof(Person.Friends));
            HasMany<PersonResource>( nameof( Person.FamilyMembers));
        }
    }
}