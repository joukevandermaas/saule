namespace Saule.Common.Tests.Models
{
    public class CompanyWithCustomersResource : CompanyResource
    {
        public CompanyWithCustomersResource()
        {
            HasMany<CustomerResource>(nameof(CompanyWithCustomers.Customers));
        }
    }
}
