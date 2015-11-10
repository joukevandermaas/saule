using Saule;

namespace Tests.Models
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