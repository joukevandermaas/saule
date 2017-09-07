using Saule;

namespace Tests.Models
{
    public class CompanyResource : ApiResource
    {
        public CompanyResource()
        {
            OfType("Corporation");
            Attribute(nameof(Company.Name));
            Attribute(nameof(Company.NumberOfEmployees));
            Attribute(nameof(Company.Location));
        }
    }
}