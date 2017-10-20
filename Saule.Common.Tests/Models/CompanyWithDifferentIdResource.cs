namespace Saule.Common.Tests.Models
{
    public class CompanyWithDifferentIdResource : CompanyResource
    {
        public CompanyWithDifferentIdResource()
        {
            WithId("CompanyId");
        }
    }
}