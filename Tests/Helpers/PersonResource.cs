using Saule;

namespace Tests.Helpers
{
    public class PersonResource : ApiResource
    {
        public PersonResource()
        {
            Attribute("FirstName");
            Attribute("LastName");
            Attribute("Age");

            BelongsTo<CompanyResource>("Job", "/employer");
            HasMany<PersonResource>("Friends");
        }
    }
}