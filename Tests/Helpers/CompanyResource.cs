using Saule;

namespace Tests.Helpers
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