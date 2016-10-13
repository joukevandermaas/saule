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

            BelongsTo<CompanyResource>(nameof(Person.Job), "/employer");
            BelongsTo<CarResource>(nameof(Person.Car));
            HasMany<PersonResource>(nameof(Person.Friends));
            HasMany<PersonResource>( nameof( Person.FamilyMembers));
        }
    }
}