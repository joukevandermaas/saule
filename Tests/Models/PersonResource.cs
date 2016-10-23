using Saule;

namespace Tests.Models
{
    public class PersonResource : ApiResource
    {
        public PersonResource()
        {
            WithId(nameof(Person.Identifier));

            Attribute(nameof(Person.FirstName));
            Attribute(nameof(Person.LastName));
            Attribute(nameof(Person.Age));
            Attribute(nameof(Person.Address));

            BelongsTo<CompanyResource>(nameof(Person.Job), "/employer");
            HasMany<PersonResource>(nameof(Person.Friends));
            HasMany<PersonResource>( nameof( Person.FamilyMembers));
        }
    }
}