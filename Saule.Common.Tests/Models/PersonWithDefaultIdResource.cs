namespace Saule.Common.Tests.Models
{
    public class PersonWithDefaultIdResource : PersonResource
    {
        public PersonWithDefaultIdResource()
        {
            WithId("Id");
        }
    }
}