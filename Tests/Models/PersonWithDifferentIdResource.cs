using Saule;

namespace Tests.Models
{
    public class PersonWithDifferentIdResource : ApiResource
    {
        public PersonWithDifferentIdResource()
        {
            OfType("Person");
            WithId("PersonId");

            Attribute("FirstName");
            Attribute("LastName");
            Attribute("Age");

            BelongsTo<CompanyWithDifferentIdResource>("Job", "/employer");
            HasMany<PersonWithDifferentIdResource>("Friends");
        }
    }
}