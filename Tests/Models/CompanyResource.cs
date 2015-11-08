using Saule;

namespace Tests.Models
{
    public class CompanyResource : ApiResource
    {
        public CompanyResource()
        {
            OfType("Corporation");
            Attribute("Name");
            Attribute("NumberOfEmployees");
        }
    }
}