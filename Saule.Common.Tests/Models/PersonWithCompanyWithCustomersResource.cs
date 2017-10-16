namespace Saule.Common.Tests.Models
{
    public class PersonWithCompanyWithCustomersResource : ApiResource
    {
        public PersonWithCompanyWithCustomersResource()
        {
            OfType("Person");

            WithId(nameof(Person.Identifier));
            
            BelongsTo<CompanyWithCustomersResource>(nameof(Person.Job), "/employer");
        }
    }
}
